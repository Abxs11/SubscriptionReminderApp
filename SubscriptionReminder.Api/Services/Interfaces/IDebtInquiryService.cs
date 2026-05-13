using SubscriptionReminder.Api.DTOs.DebtInquiry;

namespace SubscriptionReminder.Api.Services.Interfaces;

public interface IDebtInquiryService
{
    Task<DebtInquiryDto> QueryAsync(int subscriptionId);
    Task<List<DebtInquiryDto>> GetBySubscriptionIdAsync(int subscriptionId);
}
