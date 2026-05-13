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

        // Mock dış servisten borç sorgula
        var result = await _externalService.QueryDebtAsync(
            subscription.SubscriberNumber,
            subscription.Type,
            subscription.ProviderName);

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
