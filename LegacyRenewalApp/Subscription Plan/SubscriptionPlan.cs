namespace LegacyRenewalApp
{
    public class SubscriptionPlan
    {
        public PlanCode Code { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal MonthlyPricePerSeat { get; set; }
        public decimal SetupFee { get; set; }
        public bool IsEducationEligible { get; set; }
    }
}
