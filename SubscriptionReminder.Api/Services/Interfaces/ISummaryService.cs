using SubscriptionReminder.Api.DTOs.Summary;
using SubscriptionReminder.Api.DTOs.Subscription;
using SubscriptionReminder.Api.DTOs.Payment;

namespace SubscriptionReminder.Api.Services.Interfaces;

public interface ISummaryService
{
    Task<DashboardSummaryDto> GetDashboardSummaryAsync(int customerId);
    Task<List<SubscriptionDto>> GetUnpaidSubscriptionsAsync(int customerId);
    Task<List<PaymentDto>> GetPaymentHistoryAsync(int customerId);
}
