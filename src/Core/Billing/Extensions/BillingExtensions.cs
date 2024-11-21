﻿using Bit.Core.AdminConsole.Entities;
using Bit.Core.AdminConsole.Entities.Provider;
using Bit.Core.AdminConsole.Enums.Provider;
using Bit.Core.Billing.Enums;
using Bit.Core.Entities;
using Bit.Core.Enums;
using Stripe;

namespace Bit.Core.Billing.Extensions;

public static class BillingExtensions
{
    public static bool IsBillable(this Provider provider) =>
        provider.SupportsConsolidatedBilling() && provider.Status == ProviderStatusType.Billable;

    public static bool SupportsConsolidatedBilling(this Provider provider)
        => provider.Type.SupportsConsolidatedBilling();

    public static bool SupportsConsolidatedBilling(this ProviderType providerType)
        => providerType is ProviderType.Msp or ProviderType.MultiOrganizationEnterprise;

    public static bool IsValidClient(this Organization organization)
        => organization is
        {
            Seats: not null,
            Status: OrganizationStatusType.Managed,
            PlanType: PlanType.TeamsMonthly or PlanType.EnterpriseMonthly
        };

    public static bool IsStripeEnabled(this ISubscriber subscriber)
        => !string.IsNullOrEmpty(subscriber.GatewayCustomerId) &&
           !string.IsNullOrEmpty(subscriber.GatewaySubscriptionId);

    public static bool IsUnverifiedBankAccount(this SetupIntent setupIntent) =>
        setupIntent is
        {
            Status: "requires_action",
            NextAction:
            {
                VerifyWithMicrodeposits: not null
            },
            PaymentMethod:
            {
                UsBankAccount: not null
            }
        };

    public static bool SupportsConsolidatedBilling(this PlanType planType)
        => planType is PlanType.TeamsMonthly or PlanType.EnterpriseMonthly or PlanType.EnterpriseAnnually;
}
