namespace SubscriptionReminder.Api.DTOs.Card;

public class SavedCardDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CardHolderName { get; set; } = string.Empty;
    public string MaskedCardNumber { get; set; } = string.Empty;
    public string ExpiryDate { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
}
