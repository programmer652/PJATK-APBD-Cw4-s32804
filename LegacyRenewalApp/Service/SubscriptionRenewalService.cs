using System;

namespace LegacyRenewalApp.Service
{
    public class SubscriptionRenewalService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ISubscriptionPlanRepository _planRepository;
        private readonly IEmailService _emailService;
        private readonly RequestParser _parser; // interface not needed because won't change

        public SubscriptionRenewalService(ICustomerRepository customerRepository, ISubscriptionPlanRepository planRepository,  IEmailService emailService)
        {
            _customerRepository = customerRepository;
            _planRepository = planRepository;
            _emailService = emailService;
            _parser = new RequestParser();
        }
        
        public RenewalInvoice CreateRenewalInvoice(
            int customerId,
            string planCode,
            int seatCount,
            string paymentMethod,
            bool includePremiumSupport,
            bool useLoyaltyPoints)
        {
            new ValidationService().Validate(customerId, planCode, seatCount, paymentMethod); // interface not needed because won't change
            
            var pMethod = _parser.ParsePaymentMethod(paymentMethod);
            var pCode = _parser.ParsePlanCode(planCode);

            var customer = _customerRepository.GetById(customerId);
            var plan = _planRepository.GetByCode(pCode);

            customer.CheckActive();
            
            Discount discount = new Discount(plan, customer, seatCount);
            string notes = string.Empty;
            
            notes += discount.SegmentDiscount();
            notes += discount.LoyaltyDiscount();
            notes += discount.TeamSizeDiscount();
            notes += discount.UseLoyaltyPoints(useLoyaltyPoints);
            notes += discount.SubtotalAfter();
            
            Fee fee = new Fee(plan,pMethod);

            notes += fee.CalculateSupportFee(includePremiumSupport);
            fee.CalculatePaymentFee(discount);

            notes += fee.GetPaymentNote();

            var tax = new Tax(customer,discount,fee);
            
            tax.CalculateTaxRate();
            notes += tax.CalculateFinalAmount();
            
            var invoice = new RenewalInvoice
            {
                InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{customerId}-{plan.Code}",
                CustomerName = customer.FullName,
                PlanCode = plan.Code,
                PaymentMethod = pMethod,
                SeatCount = seatCount,
                BaseAmount = Math.Round(discount.baseAmount, 2, MidpointRounding.AwayFromZero),
                DiscountAmount = Math.Round(discount.amount, 2, MidpointRounding.AwayFromZero),
                SupportFee = Math.Round(fee.supportFee, 2, MidpointRounding.AwayFromZero),
                PaymentFee = Math.Round(fee.paymentFee, 2, MidpointRounding.AwayFromZero),
                TaxAmount = Math.Round(tax.taxAmount, 2, MidpointRounding.AwayFromZero),
                FinalAmount = Math.Round(tax.finalAmount, 2, MidpointRounding.AwayFromZero),
                Notes = notes.Trim(),
                GeneratedAt = DateTime.UtcNow
            };
            
            _emailService.SaveInvoice(invoice);

            if (!string.IsNullOrWhiteSpace(customer.Email))
            {
               _emailService.SendEmail(customer, invoice, plan);
            }

            return invoice;
        }
        
        
    }
}
