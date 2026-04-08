namespace LegacyRenewalApp;

public interface ISubscriptionPlanRepository
{
    SubscriptionPlan GetByCode(PlanCode planCode);
}