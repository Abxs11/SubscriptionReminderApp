namespace SubscriptionReminder.Api.Models;

public class Payment
{
    public int Id { get; set; }
    public int SubscriptionId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDateUtc { get; set; } = DateTime.UtcNow;
    public string Period { get; set; } = string.Empty; // e.g. "2026-05"
    public DateOnly DueDate { get; set; }
    public string Status { get; set; } = string.Empty; // Success, Failed
    public string? ExternalTransactionId { get; set; }
    public string? FailureReason { get; set; }

    public Subscription Subscription { get; set; } = null!;
}
