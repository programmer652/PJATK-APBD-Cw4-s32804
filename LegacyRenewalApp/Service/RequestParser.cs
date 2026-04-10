using System;

namespace LegacyRenewalApp.Service;

public class RequestParser
{
    public PaymentMethod ParsePaymentMethod(string paymentMethod)
    {
        if (!Enum.TryParse(paymentMethod?.Trim(), true, out PaymentMethod result))
        {
            throw new ArgumentException("Unsupported payment method");
        }

        return result;
    }

    public PlanCode ParsePlanCode(string planCode)
    {
        if (!Enum.TryParse(planCode?.Trim(), true, out PlanCode result))
        {
            throw new ArgumentException("Unsupported plan code");
        }

        return result;
    }
}