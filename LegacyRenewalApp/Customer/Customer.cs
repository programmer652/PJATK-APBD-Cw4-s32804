using System;

namespace LegacyRenewalApp
{
    public class Customer
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public SegmentType Segment { get; set; }
        public string Country { get; set; } = string.Empty;
        public int YearsWithCompany { get; set; }
        public int LoyaltyPoints { get; set; }
        public bool IsActive { get; set; }

        public void CheckActive()
        {
            if (!IsActive)
            {
                throw new InvalidOperationException("Inactive customers cannot renew subscriptions");
            }
        }
    }
}