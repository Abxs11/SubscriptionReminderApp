using System.ComponentModel.DataAnnotations;

namespace SubscriptionReminder.Api.DTOs.Card;

public class CreateSavedCardRequest
{
    [Required]
    public int CustomerId { get; set; }

    [Required]
    [MaxLength(150)]
    public string CardHolderName { get; set; } = string.Empty;

    [Required]
    [StringLength(16, MinimumLength = 16)]
    public string FullCardNumber { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^(0[1-9]|1[0-2])\/?([0-9]{2})$")]
    public string ExpiryDate { get; set; } = string.Empty;

    // We do not even ask for CVV for this simulation or we could ask and just ignore it.
    // Let's ask for it to simulate real world, but we will ignore it.
    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string Cvv { get; set; } = string.Empty;
}
