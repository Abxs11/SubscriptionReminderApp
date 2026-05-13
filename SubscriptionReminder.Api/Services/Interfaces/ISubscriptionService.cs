using SubscriptionReminder.Api.DTOs.Subscription;

namespace SubscriptionReminder.Api.Services.Interfaces;

public interface ISubscriptionService
{
    Task<SubscriptionDto> CreateAsync(CreateSubscriptionRequest request);
    Task<List<SubscriptionDto>> GetAllByCustomerIdAsync(int customerId);
    Task<SubscriptionDto?> GetByIdAsync(int id);
    Task<SubscriptionDto?> UpdateAsync(int id, UpdateSubscriptionRequest request);
    Task<bool> DeleteAsync(int id);
}
