using SubscriptionReminder.Api.DTOs.DebtInquiry;

namespace SubscriptionReminder.Api.Services.Interfaces;

public interface IDebtInquiryService
{
    Task<DebtInquiryDto> QueryAsync(int subscriptionId);
    Task<DebtStatusDto> GetStatusForPeriodAsync(int subscriptionId, string period);
    Task<List<DebtInquiryDto>> GetBySubscriptionIdAsync(int subscriptionId);
}
