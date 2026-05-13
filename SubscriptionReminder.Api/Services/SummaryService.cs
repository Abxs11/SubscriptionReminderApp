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

    public SummaryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(int customerId)
    {
        var currentPeriod = DateTime.UtcNow.ToString("yyyy-MM");

        var subscriptions = await _context.Subscriptions
            .Where(s => s.CustomerId == customerId)
            .ToListAsync();

        var activeCount = subscriptions.Count(s => s.Status == "Active");

        var paymentsThisMonth = await _context.Payments
            .Where(p => p.Subscription.CustomerId == customerId && p.Period == currentPeriod && p.Status == "Success")
            .ToListAsync();

        var paidSubscriptionIds = paymentsThisMonth.Select(p => p.SubscriptionId).Distinct().ToList();
        var unpaidCount = subscriptions.Count(s => s.Status == "Active" && !paidSubscriptionIds.Contains(s.Id));

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

    public async Task<List<SubscriptionDto>> GetUnpaidSubscriptionsAsync(int customerId)
    {
        var currentPeriod = DateTime.UtcNow.ToString("yyyy-MM");

        var activeSubscriptions = await _context.Subscriptions
            .Where(s => s.CustomerId == customerId && s.Status == "Active")
            .ToListAsync();

        var paidSubscriptionIds = await _context.Payments
            .Where(p => p.Subscription.CustomerId == customerId && p.Period == currentPeriod && p.Status == "Success")
            .Select(p => p.SubscriptionId)
            .ToListAsync();

        return activeSubscriptions
            .Where(s => !paidSubscriptionIds.Contains(s.Id))
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
                Amount = p.Amount,
                PaymentDateUtc = p.PaymentDateUtc,
                Period = p.Period,
                Status = p.Status,
                ExternalTransactionId = p.ExternalTransactionId,
                FailureReason = p.FailureReason
            }).ToListAsync();
    }
}
