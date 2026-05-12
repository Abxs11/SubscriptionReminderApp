namespace SubscriptionReminder.Api.Models;

public class Subscription
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string Type { get; set; } = string.Empty; // Electricity, Water, Internet, Gsm, NaturalGas
    public string ProviderName { get; set; } = string.Empty;
    public string SubscriberNumber { get; set; } = string.Empty;
    public string Status { get; set; } = "Active"; // Active, Passive
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Customer Customer { get; set; } = null!;
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<DebtInquiry> DebtInquiries { get; set; } = new List<DebtInquiry>();
}
