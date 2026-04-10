using System;

namespace LegacyRenewalApp;

public class Fee
{
    private readonly SubscriptionPlan _plan;
    private readonly PaymentMethod _pMethod;
    public decimal supportFee { get; set; } = 0m;
    public decimal paymentFee { get; set; } = 0m;

    
    public Fee(SubscriptionPlan plan, PaymentMethod pMethod)
    {
        this._plan = plan;
        this._pMethod = pMethod;
    }
    
    public string CalculateSupportFee(bool includePremiumSupport)
    {
        if (includePremiumSupport)
        {
            supportFee = _plan.Code switch
            {
                PlanCode.START => 250m,
                PlanCode.PRO => 400m,
                PlanCode.ENTERPRISE => 700m,
                _ => 0m
            };

            return "premium support included; ";
        }
        return "";
    }

    public void CalculatePaymentFee(Discount discount)
    {
        paymentFee = _pMethod switch
        {
            PaymentMethod.CARD => (discount.subtotal + supportFee) * 0.02m,
            PaymentMethod.BANK_TRANSFER => (discount.subtotal + supportFee) * 0.01m,
            PaymentMethod.PAYPAL => (discount.subtotal + supportFee) * 0.035m,
            PaymentMethod.INVOICE => 0m,
            _ => throw new ArgumentException("Unsupported payment method")
        };
    }
    public string GetPaymentNote()
    {
        return _pMethod switch
        {
            PaymentMethod.CARD => "card payment fee; ",
            PaymentMethod.BANK_TRANSFER => "bank transfer fee; ",
            PaymentMethod.PAYPAL => "paypal fee; ",
            PaymentMethod.INVOICE => "invoice payment; ",
            _ => ""
        };
    }
}