namespace SubscriptionReminder.Api.Models;

public class User
{
    public int Id { get; set; }
    public int? CustomerId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // "Admin" or "Customer"
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Customer? Customer { get; set; }
}
