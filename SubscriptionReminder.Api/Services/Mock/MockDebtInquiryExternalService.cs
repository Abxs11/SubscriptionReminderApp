using SubscriptionReminder.Api.Services.Interfaces;

namespace SubscriptionReminder.Api.Services.Mock;

/// <summary>
/// Mock borç sorgulama servisi.
/// Gerçek üçüncü parti servis yerine deterministik borç bilgisi üretir.
/// Aynı abone numarası + aynı dönem için her zaman aynı sonucu döndürür.
/// </summary>
public class MockDebtInquiryExternalService : IDebtInquiryExternalService
{
    public Task<DebtInquiryResult> QueryDebtAsync(string subscriberNumber, string subscriptionType, string providerName)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var currentPeriod = $"{today.Year}-{today.Month:D2}";

        // Aynı abone + aynı dönem için her zaman aynı sonucu üretmek adına
        // deterministik bir seed kullanıyoruz.
        var seed = $"{subscriberNumber}-{currentPeriod}".GetHashCode();
        var random = new Random(seed);

        // Abonelik türüne göre farklı fiyat aralıkları
        var (minAmount, maxAmount) = subscriptionType switch
        {
            "Electricity" => (80m, 350m),
            "Water" => (30m, 120m),
            "Internet" => (100m, 250m),
            "Gsm" => (50m, 200m),
            "NaturalGas" => (100m, 500m),
            _ => (50m, 200m)
        };

        var amount = Math.Round(minAmount + (decimal)random.NextDouble() * (maxAmount - minAmount), 2);
        var dueDate = today.AddDays(random.Next(5, 20));

        var result = new DebtInquiryResult
        {
            Amount = amount,
            DueDate = dueDate,
            Period = currentPeriod,
            ExternalReference = $"REF-{subscriberNumber}-{currentPeriod}",
            HasDebt = true
        };

        return Task.FromResult(result);
    }
}
