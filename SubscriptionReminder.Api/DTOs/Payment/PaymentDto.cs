namespace SubscriptionReminder.Api.DTOs.Payment;

public class PaymentDto
{
    public int Id { get; set; }
    public int SubscriptionId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDateUtc { get; set; }
    public string Period { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ExternalTransactionId { get; set; }
    public string? FailureReason { get; set; }
}
