namespace SubscriptionReminder.Api.DTOs.DebtInquiry;

public class DebtStatusDto
{
    public bool IsPaid { get; set; }
    public decimal Amount { get; set; }
    public DateOnly DueDate { get; set; }
    public string Period { get; set; } = string.Empty;
    public DateTime? PaymentDate { get; set; }
}
