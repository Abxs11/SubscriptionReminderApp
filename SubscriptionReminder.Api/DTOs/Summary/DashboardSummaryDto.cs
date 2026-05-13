using SubscriptionReminder.Api.DTOs.Subscription;
using SubscriptionReminder.Api.DTOs.Payment;

namespace SubscriptionReminder.Api.DTOs.Summary;

public class DashboardSummaryDto
{
    public int TotalSubscriptions { get; set; }
    public int ActiveSubscriptions { get; set; }
    public int UnpaidSubscriptionsThisMonth { get; set; }
    public decimal TotalPaidThisMonth { get; set; }
    public List<SubscriptionDto> RecentSubscriptions { get; set; } = new();
    public List<PaymentDto> RecentPayments { get; set; } = new();
}
