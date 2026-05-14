using Microsoft.EntityFrameworkCore;
using SubscriptionReminder.Api.Data;
using SubscriptionReminder.Api.Models;
using SubscriptionReminder.Api.Services.Interfaces;

namespace SubscriptionReminder.Api.BackgroundServices;

public class ReminderBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ReminderBackgroundService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(10); // Test için 10 saniyede bir çalışsın

    public ReminderBackgroundService(IServiceProvider serviceProvider, ILogger<ReminderBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Hatırlatıcı Servisi başlatıldı.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var externalService = scope.ServiceProvider.GetRequiredService<IDebtInquiryExternalService>();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                    await ProcessRemindersAsync(context, externalService, emailService, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hatırlatıcı servisinde bir hata oluştu.");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    private async Task ProcessRemindersAsync(AppDbContext context, IDebtInquiryExternalService externalService, IEmailService emailService, CancellationToken stoppingToken)
    {
        var activeSubscriptions = await context.Subscriptions
            .Include(s => s.Customer)
            .Where(s => s.Status == "Active")
            .ToListAsync(stoppingToken);

        var today = DateTime.UtcNow.Date;

        foreach (var sub in activeSubscriptions)
        {
            var startMonth = new DateTime(sub.CreatedAtUtc.Year, sub.CreatedAtUtc.Month, 1);
            var currentMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

            var paidPeriods = await context.Payments
                .Where(p => p.SubscriptionId == sub.Id && p.Status == "Success")
                .Select(p => p.Period)
                .ToListAsync(stoppingToken);

            var tempMonth = startMonth;
            while (tempMonth <= currentMonth)
            {
                var periodStr = tempMonth.ToString("yyyy-MM");
                if (!paidPeriods.Contains(periodStr))
                {
                    // Bu abonelik + bu dönem için bugün zaten hatırlatma yapıldı mı?
                    var alreadySentToday = await context.ReminderLogs
                        .AnyAsync(l => l.SubscriptionId == sub.Id && l.Period == periodStr && l.SentAtUtc.Date == today, stoppingToken);

                    if (!alreadySentToday)
                    {
                        // Borç bilgisini sorgula (Mock)
                        var debt = await externalService.QueryDebtAsync(sub.SubscriberNumber, sub.Type, sub.ProviderName, periodStr);

                        // Son ödeme tarihine 10 günden fazla varsa mail atma
                        var dueDate = debt.DueDate.ToDateTime(TimeOnly.MinValue);
                        var daysUntilDue = (dueDate - DateTime.UtcNow.Date).TotalDays;

                        //if (daysUntilDue > 10)
                        //{
                        //    _logger.LogInformation("Hatırlatma atlanıyor, son ödeme tarihine {Days} gün var: {Provider}", (int)daysUntilDue, sub.ProviderName);
                        //    continue;
                        //}

                        var customerName = $"{sub.Customer.FirstName} {sub.Customer.LastName}";

                        // Mail içeriği (HTML)
                        string mailBody = $@"
                            <div style='font-family: sans-serif; padding: 20px; color: #333;'>
                                <h2 style='color: #8b5cf6;'>Ödeme Hatırlatması 💸</h2>
                                <p>Sayın <strong>{customerName}</strong>,</p>
                                <p><strong>{sub.ProviderName}</strong> ({sub.Type}) aboneliğinize ait <strong>{periodStr}</strong> dönemi borcunuz henüz ödenmemiştir.</p>
                                <div style='background: #f3f4f6; padding: 15px; border-radius: 8px; margin: 20px 0;'>
                                    <p style='margin: 5px 0;'><strong>Borç Tutarı:</strong> ₺{debt.Amount:N2}</p>
                                    <p style='margin: 5px 0;'><strong>Son Ödeme Tarihi:</strong> {debt.DueDate:dd.MM.yyyy}</p>
                                </div>
                                <p>Ödemenizi yapmak için lütfen uygulamamıza giriş yapın.</p>
                                <hr style='border: 0; border-top: 1px solid #eee; margin: 20px 0;' />
                                <p style='font-size: 0.8rem; color: #999;'>Bu otomatik bir mesajdır, lütfen yanıtlamayınız.</p>
                            </div>";

                        // Mail Gönder
                        await emailService.SendEmailAsync(sub.Customer.Email, $"{sub.ProviderName} - Ödeme Hatırlatması", mailBody);

                        // Hatırlatmayı gönder (Simüle et logger'da kalsın)
                        _logger.LogInformation("HATIRLATMA GÖNDERİLDİ: {Customer}, {Provider} ({Period})",
                            customerName,
                            sub.ProviderName,
                            periodStr);

                        // Günlüğe kaydet
                        context.ReminderLogs.Add(new ReminderLog
                        {
                            SubscriptionId = sub.Id,
                            Period = periodStr,
                            SentAtUtc = DateTime.UtcNow,
                            Status = "Sent",
                            Message = $"₺{debt.Amount} tutarındaki {periodStr} dönemi borcu için mail gönderildi."
                        });

                        await context.SaveChangesAsync(stoppingToken);
                    }
                }
                tempMonth = tempMonth.AddMonths(1);
            }
        }
    }
}
