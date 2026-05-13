namespace SubscriptionReminder.Api.Services.Interfaces;

/// <summary>
/// Mock üçüncü parti ödeme servisi.
/// Gerçek bir ödeme altyapısı yerine sahte (mock) sonuç döndürür.
/// </summary>
public interface IPaymentExternalService
{
    /// <summary>
    /// Mock ödeme işlemi gerçekleştirir. Rastgele başarılı/başarısız döndürür.
    /// </summary>
    Task<PaymentExternalResult> ProcessPaymentAsync(decimal amount, string subscriberNumber, string period);
}

public class PaymentExternalResult
{
    public bool IsSuccess { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string? FailureReason { get; set; }
}
