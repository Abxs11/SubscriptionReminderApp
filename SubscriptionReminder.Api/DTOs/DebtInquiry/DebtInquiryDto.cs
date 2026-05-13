namespace SubscriptionReminder.Api.DTOs.DebtInquiry;

public class DebtInquiryDto
{
    public int Id { get; set; }
    public int SubscriptionId { get; set; }
    public decimal Amount { get; set; }
    public DateOnly DueDate { get; set; }
    public string Period { get; set; } = string.Empty;
    public DateTime QueriedAtUtc { get; set; }
    public string ExternalReference { get; set; } = string.Empty;
}
