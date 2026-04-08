namespace LegacyRenewalApp;

public class Discount
{
    private SubscriptionPlan plan;
    private Customer customer;
    private int seatCount;
    
    public decimal amount { get; set; } = 0m;
    public decimal baseAmount { get; set; } = 0m;
    public decimal loyaltyDiscountRate { get; set; } = 0m;
    public decimal teamDiscountRate { get; set; } = 0m;
    public decimal pointsToUse { get; set; } = 0m;
    public decimal subtotal { get; set; } = 0m;
    public Discount(SubscriptionPlan  plan, Customer customer, int seatCount )
    {
        this.seatCount = seatCount;
        this.plan = plan;
        this.customer = customer;
        baseAmount = (plan.MonthlyPricePerSeat * seatCount * 12m) + plan.SetupFee;
    }
    
    public bool SegmentDiscount()
    {
        var discountRate = customer.Segment switch
        {
            SegmentType.Silver => 0.05m,
            SegmentType.Gold => 0.10m,
            SegmentType.Platinum => 0.15m,
            SegmentType.Education when plan.IsEducationEligible => 0.20m,
            _ => 0m
        };
        if (discountRate > 0m) amount += baseAmount * discountRate;
        return discountRate > 0m;
    }

    public bool LoyaltyDiscount()
    {
        loyaltyDiscountRate = customer.YearsWithCompany switch
        {
            >= 5 => 0.07m,
            >= 2 => 0.03m,
            _ => 0m
        };
        if (loyaltyDiscountRate > 0m)  amount += baseAmount * loyaltyDiscountRate;
        return loyaltyDiscountRate > 0m;
    }

    public bool TeamSizeDiscount()
    {
        teamDiscountRate = seatCount switch
        {
            >= 50 => 0.12m,
            >= 20 => 0.08m,
            >= 10 => 0.04m,
            _ => 0m
        };
        if(teamDiscountRate>0) amount += baseAmount * teamDiscountRate;
        return teamDiscountRate > 0m;
    }

    public bool UseLoyaltyPoints(bool useLoyaltyPoints)
    {
        if (useLoyaltyPoints && customer.LoyaltyPoints > 0)
        {
            pointsToUse = customer.LoyaltyPoints > 200 ? 200 : customer.LoyaltyPoints;
            amount += pointsToUse;
        }
        return pointsToUse>0;
    }

    public bool SubtotalAfter()
    {
        subtotal = baseAmount - amount;
        if (subtotal < 300m)
            subtotal = 300m;
        return baseAmount - amount < 300m;
    }
}