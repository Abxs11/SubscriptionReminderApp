using SubscriptionReminder.Api.Services.Interfaces;

namespace SubscriptionReminder.Api.Services.Mock;

/// <summary>
/// Mock ödeme servisi.
/// Gerçek ödeme altyapısı yerine %90 başarı oranıyla sahte sonuç döndürür.
/// </summary>
public class MockPaymentExternalService : IPaymentExternalService
{
    private readonly Random _random = new();

    public Task<PaymentExternalResult> ProcessPaymentAsync(decimal amount, string subscriberNumber, string period)
    {
        // %90 başarı oranı simülasyonu
        var isSuccess = _random.NextDouble() < 0.9;

        var result = new PaymentExternalResult
        {
            IsSuccess = isSuccess,
            TransactionId = $"TXN-{Guid.NewGuid().ToString()[..12].ToUpper()}",
            FailureReason = isSuccess ? null : "Yetersiz bakiye (mock hata)"
        };

        return Task.FromResult(result);
    }
}
