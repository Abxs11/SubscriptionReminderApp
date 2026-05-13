using Microsoft.EntityFrameworkCore;
using SubscriptionReminder.Api.Data;
using SubscriptionReminder.Api.DTOs.Summary;
using SubscriptionReminder.Api.DTOs.Subscription;
using SubscriptionReminder.Api.DTOs.Payment;
using SubscriptionReminder.Api.Services.Interfaces;

namespace SubscriptionReminder.Api.Services;

public class SummaryService : ISummaryService
{
    private readonly AppDbContext _context;
    private readonly IDebtInquiryExternalService _externalService;

    public SummaryService(AppDbContext context, IDebtInquiryExternalService externalService)
    {
        _context = context;
        _externalService = externalService;
    }

    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(int customerId)
    {
        var subscriptions = await _context.Subscriptions
            .Where(s => s.CustomerId == customerId)
            .ToListAsync();

        var activeCount = subscriptions.Count(s => s.Status == "Active");

        var currentPeriod = DateTime.UtcNow.ToString("yyyy-MM");

        var paymentsThisMonth = await _context.Payments
            .Where(p => p.Subscription.CustomerId == customerId && p.Period == currentPeriod && p.Status == "Success")
            .ToListAsync();

        var unpaidCount = 0;
        foreach (var sub in subscriptions.Where(s => s.Status == "Active"))
        {
            var startMonth = new DateTime(sub.CreatedAtUtc.Year, sub.CreatedAtUtc.Month, 1);
            var currentMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            
            var paidPeriods = await _context.Payments
                .Where(p => p.SubscriptionId == sub.Id && p.Status == "Success")
                .Select(p => p.Period)
                .ToListAsync();

            var tempMonth = startMonth;
            while (tempMonth <= currentMonth)
            {
                var periodStr = tempMonth.ToString("yyyy-MM");
                if (!paidPeriods.Contains(periodStr))
                {
                    unpaidCount++;
                    break; // En az bir ödenmemiş ay varsa sayalım
                }
                tempMonth = tempMonth.AddMonths(1);
            }
        }

        var recentSubscriptions = subscriptions
            .OrderByDescending(s => s.CreatedAtUtc)
            .Take(5)
            .Select(s => new SubscriptionDto
            {
                Id = s.Id,
                CustomerId = s.CustomerId,
                Type = s.Type,
                ProviderName = s.ProviderName,
                SubscriberNumber = s.SubscriberNumber,
                Status = s.Status,
                CreatedAtUtc = s.CreatedAtUtc
            }).ToList();

        var recentPayments = await _context.Payments
            .Where(p => p.Subscription.CustomerId == customerId)
            .OrderByDescending(p => p.PaymentDateUtc)
            .Take(5)
            .Select(p => new PaymentDto
            {
                Id = p.Id,
                SubscriptionId = p.SubscriptionId,
                ProviderName = p.Subscription.ProviderName,
                SubscriberNumber = p.Subscription.SubscriberNumber,
                Amount = p.Amount,
                PaymentDateUtc = p.PaymentDateUtc,
                Period = p.Period,
                Status = p.Status,
                ExternalTransactionId = p.ExternalTransactionId,
                FailureReason = p.FailureReason
            }).ToListAsync();

        return new DashboardSummaryDto
        {
            TotalSubscriptions = subscriptions.Count,
            ActiveSubscriptions = activeCount,
            UnpaidSubscriptionsThisMonth = unpaidCount,
            TotalPaidThisMonth = paymentsThisMonth.Sum(p => p.Amount),
            RecentSubscriptions = recentSubscriptions,
            RecentPayments = recentPayments
        };
    }

    public async Task<List<UnpaidSubscriptionDto>> GetUnpaidSubscriptionsAsync(int customerId)
    {
        var activeSubscriptions = await _context.Subscriptions
            .Where(s => s.CustomerId == customerId && s.Status == "Active")
            .ToListAsync();

        var unpaidList = new List<UnpaidSubscriptionDto>();

        foreach (var sub in activeSubscriptions)
        {
            var startMonth = new DateTime(sub.CreatedAtUtc.Year, sub.CreatedAtUtc.Month, 1);
            var currentMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

            var paidPeriods = await _context.Payments
                .Where(p => p.SubscriptionId == sub.Id && p.Status == "Success")
                .Select(p => p.Period)
                .ToListAsync();

            var tempMonth = startMonth;
            while (tempMonth <= currentMonth)
            {
                var periodStr = tempMonth.ToString("yyyy-MM");
                if (!paidPeriods.Contains(periodStr))
                {
                    // Her ödenmemiş ay için mock borç bilgisini al
                    var debt = await _externalService.QueryDebtAsync(sub.SubscriberNumber, sub.Type, sub.ProviderName, periodStr);
                    
                    unpaidList.Add(new UnpaidSubscriptionDto
                    {
                        Id = sub.Id,
                        CustomerId = sub.CustomerId,
                        Type = sub.Type,
                        ProviderName = sub.ProviderName,
                        SubscriberNumber = sub.SubscriberNumber,
                        Status = sub.Status,
                        CreatedAtUtc = sub.CreatedAtUtc,
                        Period = periodStr,
                        Amount = debt.Amount,
                        DueDate = debt.DueDate
                    });
                }
                tempMonth = tempMonth.AddMonths(1);
            }
        }

        return unpaidList.OrderBy(x => x.Period).ToList();
    }

    public async Task<List<PaymentDto>> GetPaymentHistoryAsync(int customerId)
    {
        return await _context.Payments
            .Where(p => p.Subscription.CustomerId == customerId)
            .OrderByDescending(p => p.PaymentDateUtc)
            .Select(p => new PaymentDto
            {
                Id = p.Id,
                SubscriptionId = p.SubscriptionId,
                ProviderName = p.Subscription.ProviderName,
                SubscriberNumber = p.Subscription.SubscriberNumber,
                Amount = p.Amount,
                PaymentDateUtc = p.PaymentDateUtc,
                Period = p.Period,
                Status = p.Status,
                ExternalTransactionId = p.ExternalTransactionId,
                FailureReason = p.FailureReason
            }).ToListAsync();
    }
}
