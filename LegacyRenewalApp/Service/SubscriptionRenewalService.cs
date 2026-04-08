using System;
using System.Collections.Generic;

namespace LegacyRenewalApp
{
    public class SubscriptionRenewalService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ISubscriptionPlanRepository _planRepository;

        public SubscriptionRenewalService(ICustomerRepository customerRepository, ISubscriptionPlanRepository planRepository)
        {
            _customerRepository = customerRepository;
            _planRepository = planRepository;
        }
        
        public RenewalInvoice CreateRenewalInvoice(
            int customerId,
            string planCode,
            int seatCount,
            string paymentMethod,
            bool includePremiumSupport,
            bool useLoyaltyPoints)
        {
            new ValidationService().Validate(customerId, planCode, seatCount, paymentMethod);
            
            var pMethod = (PaymentMethod)Enum.Parse(typeof(PaymentMethod), paymentMethod.Trim().ToUpperInvariant());
            var pCode = (PlanCode)Enum.Parse(typeof(PlanCode), planCode.Trim().ToUpperInvariant());
            var customer = _customerRepository.GetById(customerId);
            var plan = _planRepository.GetByCode(pCode);

            customer.CheckActive();


            Discount discount = new Discount(plan, customer, seatCount);
            string notes = string.Empty;
            
            

            if (discount.SegmentDiscount())
                notes += $"{customer.Segment.ToString().ToLower()} discount; ";
            
            
            if (discount.LoyaltyDiscount())
                notes += discount.loyaltyDiscountRate == 0.07m ? "long-term loyalty discount; " : "basic loyalty discount; ";
            
            
            if (discount.TeamSizeDiscount())
                notes += discount.teamDiscountRate switch
                {
                    0.12m => "large team discount; ",
                    0.08m => "medium team discount; ",
                    0.04m => "small team discount; ",
                    _ => ""
                };

            if (discount.UseLoyaltyPoints(useLoyaltyPoints))
                notes += $"loyalty points used: {discount.pointsToUse}; ";
            
            
            if (discount.SubtotalAfter())
                notes += "minimum discounted subtotal applied; ";
            

            decimal supportFee = 0m;

            if (includePremiumSupport)
            {
                supportFee = plan.Code switch
                {
                    PlanCode.START => 250m,
                    PlanCode.PRO => 400m,
                    PlanCode.ENTERPRISE => 700m,
                    _ => 0m
                };

                notes += "premium support included; ";
            }

            decimal paymentFee = pMethod switch
            {
                PaymentMethod.CARD => (discount.subtotal + supportFee) * 0.02m,
                PaymentMethod.BANK_TRANSFER => (discount.subtotal + supportFee) * 0.01m,
                PaymentMethod.PAYPAL => (discount.subtotal + supportFee) * 0.035m,
                PaymentMethod.INVOICE => 0m,
                _ => throw new ArgumentException("Unsupported payment method")
            };
            
            notes += pMethod switch
            {
                PaymentMethod.CARD => "card payment fee; ",
                PaymentMethod.BANK_TRANSFER => "bank transfer fee; ",
                PaymentMethod.PAYPAL => "paypal fee; ",
                PaymentMethod.INVOICE => "invoice payment; ",
                _ => ""
            };

            decimal taxRate = customer.Country switch
            {
                "Poland" => 0.23m,
                "Germany" => 0.19m,
                "Czech Republic" => 0.21m,
                "Norway" => 0.25m,
                _ => 0.20m
            };

            decimal taxBase = discount.subtotal + supportFee + paymentFee;
            decimal taxAmount = taxBase * taxRate;
            decimal finalAmount = taxBase + taxAmount;

            if (finalAmount < 500m)
            {
                finalAmount = 500m;
                notes += "minimum invoice amount applied; ";
            }

            var invoice = new RenewalInvoice
            {
                InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{customerId}-{plan.Code}",
                CustomerName = customer.FullName,
                PlanCode = plan.Code,
                PaymentMethod = pMethod,
                SeatCount = seatCount,
                BaseAmount = Math.Round(discount.baseAmount, 2, MidpointRounding.AwayFromZero),
                DiscountAmount = Math.Round(discount.amount, 2, MidpointRounding.AwayFromZero),
                SupportFee = Math.Round(supportFee, 2, MidpointRounding.AwayFromZero),
                PaymentFee = Math.Round(paymentFee, 2, MidpointRounding.AwayFromZero),
                TaxAmount = Math.Round(taxAmount, 2, MidpointRounding.AwayFromZero),
                FinalAmount = Math.Round(finalAmount, 2, MidpointRounding.AwayFromZero),
                Notes = notes.Trim(),
                GeneratedAt = DateTime.UtcNow
            };

            LegacyBillingGateway.SaveInvoice(invoice);

            if (!string.IsNullOrWhiteSpace(customer.Email))
            {
                string subject = "Subscription renewal invoice";
                string body =
                    $"Hello {customer.FullName}, your renewal for plan {plan.Code} " +
                    $"has been prepared. Final amount: {invoice.FinalAmount:F2}.";

                LegacyBillingGateway.SendEmail(customer.Email, subject, body);
            }

            return invoice;
        }
        
        
    }
}
