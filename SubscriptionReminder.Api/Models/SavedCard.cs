namespace SubscriptionReminder.Api.Models;

public class SavedCard
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CardHolderName { get; set; } = string.Empty;
    public string MaskedCardNumber { get; set; } = string.Empty; // e.g. **** **** **** 1234
    public string ExpiryDate { get; set; } = string.Empty; // e.g. 12/28
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Customer Customer { get; set; } = null!;
}
