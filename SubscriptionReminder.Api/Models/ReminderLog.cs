using System.ComponentModel.DataAnnotations;
using SubscriptionReminder.Api.Models;

namespace SubscriptionReminder.Api.Models;

public class ReminderLog
{
    public int Id { get; set; }
    
    public int SubscriptionId { get; set; }
    public Subscription Subscription { get; set; } = null!;
    
    public string Period { get; set; } = string.Empty; // yyyy-MM
    public DateTime SentAtUtc { get; set; }
    public string Status { get; set; } = "Sent"; // Sent, Failed
    public string Message { get; set; } = string.Empty;
}
