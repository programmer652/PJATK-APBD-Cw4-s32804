namespace LegacyRenewalApp;

public class LegacyBillingGatewayAdapter : IEmailService
{
    public void SaveInvoice(RenewalInvoice invoice)
    {
        LegacyBillingGateway.SaveInvoice(invoice);
    }

    public void SendEmail(Customer customer, RenewalInvoice invoice, SubscriptionPlan plan)
    {
        string subject = "Subscription renewal invoice";
        string body =
            $"Hello {customer.FullName}, your renewal for plan {plan.Code} " +
            $"has been prepared. Final amount: {invoice.FinalAmount:F2}.";
        LegacyBillingGateway.SendEmail(customer.Email, subject, body);
    }
}