namespace SubscriptionReminder.Api.Services.Interfaces;

/// <summary>
/// Mock üçüncü parti borç sorgulama servisi.
/// Gerçek bir dış servis yerine sahte (mock) veri döndürür.
/// </summary>
public interface IDebtInquiryExternalService
{
    /// <summary>
    /// Abonelik bilgilerine göre mock borç bilgisi döndürür.
    /// </summary>
    Task<DebtInquiryResult> QueryDebtAsync(string subscriberNumber, string subscriptionType, string providerName, string? period = null);
}

public class DebtInquiryResult
{
    public decimal Amount { get; set; }
    public DateOnly DueDate { get; set; }
    public string Period { get; set; } = string.Empty;
    public string ExternalReference { get; set; } = string.Empty;
    public bool HasDebt { get; set; }
}
