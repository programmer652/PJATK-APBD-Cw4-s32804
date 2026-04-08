using System;

namespace LegacyRenewalApp;

public class ValidationService
{
    public void Validate(int customerId, string planCode, int seatCount, string paymentMethod)
    {
        Check(customerId > 0, "Customer id must be positive");
        Check(!string.IsNullOrWhiteSpace(planCode), "Plan code is required");
        Check(seatCount > 0, "Seat count must be positive");
        Check(!string.IsNullOrWhiteSpace(paymentMethod), "Payment method is required");
    }
    private void Check(bool condition, string message)
    {
        if (!condition) throw new ArgumentException(message);
    }
}