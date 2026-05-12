namespace SubscriptionReminder.Api.Models;

public class DebtInquiry
{
    public int Id { get; set; }
    public int SubscriptionId { get; set; }
    public decimal Amount { get; set; }
    public DateOnly DueDate { get; set; }
    public string Period { get; set; } = string.Empty; // e.g. "2026-05"
    public DateTime QueriedAtUtc { get; set; } = DateTime.UtcNow;
    public string ExternalReference { get; set; } = string.Empty;

    public Subscription Subscription { get; set; } = null!;
}
