namespace SubscriptionReminder.Api.DTOs.Subscription;

public class UpdateSubscriptionRequest
{
    public string? ProviderName { get; set; }
    public string? SubscriberNumber { get; set; }
    public string? Status { get; set; } // Active, Passive
}
