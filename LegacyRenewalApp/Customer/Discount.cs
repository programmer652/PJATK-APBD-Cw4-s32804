namespace LegacyRenewalApp;

public class Discount
{
    private SubscriptionPlan _plan;
    private Customer _customer;
    private int _seatCount;
    
    public decimal amount { get; set; } = 0m;
    public decimal baseAmount { get; set; } = 0m;
    public decimal loyaltyDiscountRate { get; set; } = 0m;
    public decimal teamDiscountRate { get; set; } = 0m;
    public decimal pointsToUse { get; set; } = 0m;
    public decimal subtotal { get; set; } = 0m;
    public Discount(SubscriptionPlan  plan, Customer customer, int seatCount )
    {
        _seatCount = seatCount;
        _plan = plan;
        _customer = customer;
        baseAmount = (plan.MonthlyPricePerSeat * seatCount * 12m) + plan.SetupFee;
    }
    
    public string SegmentDiscount()
    {
        var discountRate = _customer.Segment switch
        {
            SegmentType.Silver => 0.05m,
            SegmentType.Gold => 0.10m,
            SegmentType.Platinum => 0.15m,
            SegmentType.Education when _plan.IsEducationEligible => 0.20m,
            _ => 0m
        };
        if (discountRate > 0m) 
        {
            amount += baseAmount * discountRate;
            return $"{_customer.Segment.ToString().ToLower()} discount; ";
        }
        return "";
    }

    public string LoyaltyDiscount()
    {
        loyaltyDiscountRate = _customer.YearsWithCompany switch
        {
            >= 5 => 0.07m,
            >= 2 => 0.03m,
            _ => 0m
        };
        if (loyaltyDiscountRate > 0m)
        {
            amount += baseAmount * loyaltyDiscountRate;
            return loyaltyDiscountRate == 0.07m ? "long-term loyalty discount; " : "basic loyalty discount; ";
        }
        return "";
    }

    public string TeamSizeDiscount()
    {
        teamDiscountRate = _seatCount switch
        {
            >= 50 => 0.12m,
            >= 20 => 0.08m,
            >= 10 => 0.04m,
            _ => 0m
        };
        if (teamDiscountRate > 0m)
        {
            amount += baseAmount * teamDiscountRate;
            return teamDiscountRate switch
            {
                0.12m => "large team discount; ",
                0.08m => "medium team discount; ",
                0.04m => "small team discount; ",
                _ => ""
            };
        }
        return "";
    }

    public string UseLoyaltyPoints(bool useLoyaltyPoints)
    {
        if (useLoyaltyPoints && _customer.LoyaltyPoints > 0)
        {
            pointsToUse = _customer.LoyaltyPoints > 200 ? 200 : _customer.LoyaltyPoints;
            amount += pointsToUse;
        }
        if(pointsToUse>0)
            return $"loyalty points used: {pointsToUse}; ";
        return "";
    }

    public string SubtotalAfter()
    {
        subtotal = baseAmount - amount;
        if (subtotal < 300m)
            subtotal = 300m;
        if(baseAmount - amount < 300m)
            return "minimum discounted subtotal applied; ";
        return "";
    }
}