namespace SubscriptionReminder.Api.DTOs.Payment;

public class CreatePaymentRequest
{
    public int SubscriptionId { get; set; }
    public decimal Amount { get; set; }
    public string Period { get; set; } = string.Empty; // e.g. "2026-05"
    public DateOnly DueDate { get; set; }
}
