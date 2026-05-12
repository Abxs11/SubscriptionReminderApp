using SubscriptionReminder.Api.DTOs.Customer;

namespace SubscriptionReminder.Api.Services.Interfaces;

public interface ICustomerService
{
    Task<CustomerDto> CreateAsync(CreateCustomerRequest request);
    Task<List<CustomerDto>> GetAllAsync();
    Task<CustomerDto?> GetByIdAsync(int id);
    Task<bool> DeleteAsync(int id);
}
