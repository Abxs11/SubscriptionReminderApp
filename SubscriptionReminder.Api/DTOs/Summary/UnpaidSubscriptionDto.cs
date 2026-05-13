using SubscriptionReminder.Api.DTOs.Subscription;

namespace SubscriptionReminder.Api.DTOs.Summary;

public class UnpaidSubscriptionDto : SubscriptionDto
{
    public string Period { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateOnly DueDate { get; set; }
}
