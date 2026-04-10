namespace LegacyRenewalApp;

public class Tax
{
    private Customer customer;
    private Discount discount;
    private Fee fee;
    public decimal taxRate { get; set; } = 0m;
    public decimal taxBase { get; set; } = 0m;
    public decimal taxAmount { get; set; } = 0m;
    public decimal finalAmount { get; set; } = 0m;

    public Tax(Customer customer, Discount discount, Fee fee)
    {
        this.customer = customer;
        this.discount = discount;
        this.fee = fee;
        taxBase = discount.subtotal + fee.supportFee + fee.paymentFee;
    }

    public void CalculateTaxRate()
    {
        decimal taxRate = customer.Country switch
        {
            "Poland" => 0.23m,
            "Germany" => 0.19m,
            "Czech Republic" => 0.21m,
            "Norway" => 0.25m,
            _ => 0.20m
        };
    }

    public string CalculateFinalAmount()
    {
        decimal taxAmount = taxBase * taxRate;
        decimal finalAmount = taxBase + taxAmount;
        if (finalAmount < 500m)
        {
            finalAmount = 500m;
            return "minimum invoice amount applied; ";
        }
        return "";
    }
}