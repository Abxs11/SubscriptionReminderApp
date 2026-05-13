using SubscriptionReminder.Api.DTOs.Payment;

namespace SubscriptionReminder.Api.Services.Interfaces;

public interface IPaymentService
{
    Task<PaymentDto> CreateAsync(CreatePaymentRequest request);
    Task<List<PaymentDto>> GetBySubscriptionIdAsync(int subscriptionId);
    Task<PaymentDto?> GetByIdAsync(int id);
}
