namespace LegacyRenewalApp;

public interface IEmailService
{
    void SendEmail(Customer customer, RenewalInvoice invoice, SubscriptionPlan plan);
    void SaveInvoice(RenewalInvoice invoice);
}