namespace SubscriptionReminder.Api.DTOs.Subscription;

public class CreateSubscriptionRequest
{
    public int CustomerId { get; set; }
    public string Type { get; set; } = string.Empty;       // Electricity, Water, Internet, Gsm, NaturalGas
    public string ProviderName { get; set; } = string.Empty;
    public string SubscriberNumber { get; set; } = string.Empty;
}
