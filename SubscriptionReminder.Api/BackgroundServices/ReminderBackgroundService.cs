using Microsoft.EntityFrameworkCore;
using SubscriptionReminder.Api.Data;
using SubscriptionReminder.Api.Models;

namespace SubscriptionReminder.Api.BackgroundServices;

public class ReminderBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ReminderBackgroundService> _logger;

    public ReminderBackgroundService(IServiceProvider serviceProvider, ILogger<ReminderBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Abonelik Hatırlatma Servisi başlatıldı.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    await ProcessRemindersAsync(context);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hatırlatma servisi çalışırken hata oluştu.");
            }

            // Test amaçlı 1 dakikada bir çalışacak şekilde ayarlandı.
            // Gerçek senaryoda 24 saatte bir çalışması uygundur: TimeSpan.FromHours(24)
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task ProcessRemindersAsync(AppDbContext context)
    {
        var currentPeriod = DateTime.UtcNow.ToString("yyyy-MM");
        _logger.LogInformation("{Period} dönemi için ödenmemiş abonelikler taranıyor...", currentPeriod);

        // 1. Aktif abonelikleri getir
        var activeSubscriptions = await context.Subscriptions
            .Include(s => s.Customer)
            .Where(s => s.Status == "Active")
            .ToListAsync();

        // 2. Bu dönem için ödeme yapmış olanları bul
        var paidSubscriptionIds = await context.Payments
            .Where(p => p.Period == currentPeriod && p.Status == "Success")
            .Select(p => p.SubscriptionId)
            .ToListAsync();

        // 3. Henüz ödeme yapmamış olanları filtrele
        var unpaidSubscriptions = activeSubscriptions
            .Where(s => !paidSubscriptionIds.Contains(s.Id))
            .ToList();

        foreach (var sub in unpaidSubscriptions)
        {
            // 4. Bugün zaten hatırlatma atılmış mı kontrol et (Mükerrer olmasın)
            var alreadyReminded = await context.ReminderLogs
                .AnyAsync(r => r.SubscriptionId == sub.Id && r.Period == currentPeriod && r.SentAtUtc.Date == DateTime.UtcNow.Date);

            if (!alreadyReminded)
            {
                // Hatırlatma gönder (Simüle ediliyor)
                _logger.LogWarning("HATIRLATMA GÖNDERİLDİ: Sayın {FirstName} {LastName}, {ProviderName} ({Type}) faturanız henüz ödenmedi! Abone No: {SubNo}", 
                    sub.Customer.FirstName, sub.Customer.LastName, sub.ProviderName, sub.Type, sub.SubscriberNumber);

                // Log kaydet
                context.ReminderLogs.Add(new ReminderLog
                {
                    SubscriptionId = sub.Id,
                    Period = currentPeriod,
                    SentAtUtc = DateTime.UtcNow,
                    Status = "Sent",
                    Message = $"{sub.ProviderName} faturası için hatırlatma yapıldı."
                });
            }
        }

        await context.SaveChangesAsync();
    }
}
