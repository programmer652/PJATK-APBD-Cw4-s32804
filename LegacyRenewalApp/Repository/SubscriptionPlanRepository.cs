using System;
using System.Collections.Generic;
using System.Threading;

namespace LegacyRenewalApp
{
    public class SubscriptionPlanRepository : ISubscriptionPlanRepository
    {
        public static readonly Dictionary<string, SubscriptionPlan> Database = new Dictionary<string, SubscriptionPlan>
        {
            { "START", new SubscriptionPlan { Code = PlanCode.START, Name = "Start", MonthlyPricePerSeat = 49m, SetupFee = 120m, IsEducationEligible = false } },
            { "PRO", new SubscriptionPlan { Code = PlanCode.PRO, Name = "Professional", MonthlyPricePerSeat = 89m, SetupFee = 180m, IsEducationEligible = true } },
            { "ENTERPRISE", new SubscriptionPlan { Code = PlanCode.ENTERPRISE, Name = "Enterprise", MonthlyPricePerSeat = 149m, SetupFee = 300m, IsEducationEligible = false } }
        };

        public SubscriptionPlan GetByCode(PlanCode code)
        {
            int randomWaitTime = new Random().Next(500);
            Thread.Sleep(randomWaitTime);
            
            if (Database.ContainsKey(code.ToString()))
            {
                return Database[code.ToString()];
            }

            throw new ArgumentException($"Plan with code {code} does not exist");
        }
    }
}
