using SubscriptionReminder.Api.DTOs.Card;

namespace SubscriptionReminder.Api.Services.Interfaces;

public interface ISavedCardService
{
    Task<SavedCardDto> CreateAsync(CreateSavedCardRequest request);
    Task<List<SavedCardDto>> GetAllByCustomerIdAsync(int customerId);
    Task<bool> DeleteAsync(int id, int customerId);
}
