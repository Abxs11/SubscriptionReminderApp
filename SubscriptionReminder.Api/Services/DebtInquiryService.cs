using Microsoft.EntityFrameworkCore;
using SubscriptionReminder.Api.Data;
using SubscriptionReminder.Api.DTOs.DebtInquiry;
using SubscriptionReminder.Api.Services.Interfaces;

namespace SubscriptionReminder.Api.Services;

public class DebtInquiryService : IDebtInquiryService
{
    private readonly AppDbContext _context;
    private readonly IDebtInquiryExternalService _externalService;

    public DebtInquiryService(AppDbContext context, IDebtInquiryExternalService externalService)
    {
        _context = context;
        _externalService = externalService;
    }

    public async Task<DebtInquiryDto> QueryAsync(int subscriptionId)
    {
        var subscription = await _context.Subscriptions.FindAsync(subscriptionId);
        if (subscription == null)
            throw new KeyNotFoundException($"ID {subscriptionId} ile abonelik bulunamadı.");

        // Abonelik başlangıcından bugüne kadar olan ödenmemiş en eski ayı bul
        var startMonth = new DateTime(subscription.CreatedAtUtc.Year, subscription.CreatedAtUtc.Month, 1);
        var currentMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

        var paidPeriods = await _context.Payments
            .Where(p => p.SubscriptionId == subscriptionId && p.Status == "Success")
            .Select(p => p.Period)
            .ToListAsync();

        string? targetPeriod = null;
        var tempMonth = startMonth;
        while (tempMonth <= currentMonth)
        {
            var periodStr = tempMonth.ToString("yyyy-MM");
            if (!paidPeriods.Contains(periodStr))
            {
                targetPeriod = periodStr;
                break;
            }
            tempMonth = tempMonth.AddMonths(1);
        }

        if (targetPeriod == null)
            throw new InvalidOperationException("Bu abonelik için ödenmemiş borç bulunamadı.");

        // Mock dış servisten borç sorgula
        var result = await _externalService.QueryDebtAsync(
            subscription.SubscriberNumber,
            subscription.Type,
            subscription.ProviderName,
            targetPeriod);

        if (!result.HasDebt)
            throw new InvalidOperationException("Bu abonelik için borç bulunamadı.");

        // Sorgu sonucunu veritabanına kaydet
        var inquiry = new Models.DebtInquiry
        {
            SubscriptionId = subscriptionId,
            Amount = result.Amount,
            DueDate = result.DueDate,
            Period = result.Period,
            QueriedAtUtc = DateTime.UtcNow,
            ExternalReference = result.ExternalReference
        };

        _context.DebtInquiries.Add(inquiry);
        await _context.SaveChangesAsync();

        return MapToDto(inquiry);
    }

    public async Task<DebtStatusDto> GetStatusForPeriodAsync(int subscriptionId, string period)
    {
        var subscription = await _context.Subscriptions.FindAsync(subscriptionId);
        if (subscription == null)
            throw new KeyNotFoundException("Abonelik bulunamadı.");

        // Ödeme kontrolü
        var payment = await _context.Payments
            .FirstOrDefaultAsync(p => p.SubscriptionId == subscriptionId && p.Period == period && p.Status == "Success");

        // Mock servisten borç bilgisini al
        var debtResult = await _externalService.QueryDebtAsync(
            subscription.SubscriberNumber,
            subscription.Type,
            subscription.ProviderName,
            period);

        return new DebtStatusDto
        {
            IsPaid = payment != null,
            Amount = debtResult.Amount,
            DueDate = debtResult.DueDate,
            Period = period,
            PaymentDate = payment?.PaymentDateUtc
        };
    }

    public async Task<List<DebtInquiryDto>> GetBySubscriptionIdAsync(int subscriptionId)
    {
        var inquiries = await _context.DebtInquiries
            .AsNoTracking()
            .Where(d => d.SubscriptionId == subscriptionId)
            .OrderByDescending(d => d.QueriedAtUtc)
            .ToListAsync();

        return inquiries.Select(MapToDto).ToList();
    }

    private static DebtInquiryDto MapToDto(Models.DebtInquiry inquiry)
    {
        return new DebtInquiryDto
        {
            Id = inquiry.Id,
            SubscriptionId = inquiry.SubscriptionId,
            Amount = inquiry.Amount,
            DueDate = inquiry.DueDate,
            Period = inquiry.Period,
            QueriedAtUtc = inquiry.QueriedAtUtc,
            ExternalReference = inquiry.ExternalReference
        };
    }
}
