namespace SubscriptionReminder.Api.DTOs.Payment;

public class PaymentDto
{
    public int Id { get; set; }
    public int SubscriptionId { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public string SubscriberNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime PaymentDateUtc { get; set; }
    public string Period { get; set; } = string.Empty;
    public DateOnly DueDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ExternalTransactionId { get; set; }
    public string? FailureReason { get; set; }
}
