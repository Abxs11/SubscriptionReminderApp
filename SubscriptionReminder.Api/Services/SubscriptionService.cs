using Microsoft.EntityFrameworkCore;
using SubscriptionReminder.Api.Data;
using SubscriptionReminder.Api.DTOs.Subscription;
using SubscriptionReminder.Api.Models;
using SubscriptionReminder.Api.Services.Interfaces;

namespace SubscriptionReminder.Api.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly AppDbContext _context;

    public SubscriptionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SubscriptionDto> CreateAsync(CreateSubscriptionRequest request)
    {
        var subscription = new Subscription
        {
            CustomerId = request.CustomerId,
            Type = request.Type,
            ProviderName = request.ProviderName,
            SubscriberNumber = request.SubscriberNumber,
            Status = "Active",
            CreatedAtUtc = DateTime.UtcNow
        };

        _context.Subscriptions.Add(subscription);
        await _context.SaveChangesAsync();

        return MapToDto(subscription);
    }

    public async Task<List<SubscriptionDto>> GetAllByCustomerIdAsync(int customerId)
    {
        var subscriptions = await _context.Subscriptions
            .AsNoTracking()
            .Where(s => s.CustomerId == customerId)
            .OrderByDescending(s => s.CreatedAtUtc)
            .ToListAsync();

        return subscriptions.Select(MapToDto).ToList();
    }

    public async Task<SubscriptionDto?> GetByIdAsync(int id)
    {
        var subscription = await _context.Subscriptions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);

        return subscription == null ? null : MapToDto(subscription);
    }

    public async Task<SubscriptionDto?> UpdateAsync(int id, UpdateSubscriptionRequest request)
    {
        var subscription = await _context.Subscriptions.FindAsync(id);
        if (subscription == null)
            return null;

        if (request.ProviderName != null)
            subscription.ProviderName = request.ProviderName;

        if (request.SubscriberNumber != null)
            subscription.SubscriberNumber = request.SubscriberNumber;

        if (request.Status != null)
            subscription.Status = request.Status;

        await _context.SaveChangesAsync();

        return MapToDto(subscription);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var subscription = await _context.Subscriptions.FindAsync(id);
        if (subscription == null)
            return false;

        _context.Subscriptions.Remove(subscription);
        await _context.SaveChangesAsync();
        return true;
    }

    private static SubscriptionDto MapToDto(Subscription subscription)
    {
        return new SubscriptionDto
        {
            Id = subscription.Id,
            CustomerId = subscription.CustomerId,
            Type = subscription.Type,
            ProviderName = subscription.ProviderName,
            SubscriberNumber = subscription.SubscriberNumber,
            Status = subscription.Status,
            CreatedAtUtc = subscription.CreatedAtUtc
        };
    }
}
