using Microsoft.EntityFrameworkCore;
using SubscriptionReminder.Api.Data;
using SubscriptionReminder.Api.DTOs.Payment;
using SubscriptionReminder.Api.Models;
using SubscriptionReminder.Api.Services.Interfaces;

namespace SubscriptionReminder.Api.Services;

public class PaymentService : IPaymentService
{
    private readonly AppDbContext _context;
    private readonly IPaymentExternalService _paymentExternal;

    public PaymentService(AppDbContext context, IPaymentExternalService paymentExternal)
    {
        _context = context;
        _paymentExternal = paymentExternal;
    }

    public async Task<PaymentDto> CreateAsync(CreatePaymentRequest request)
    {
        // Aboneliğin var olup olmadığını kontrol et
        var subscription = await _context.Subscriptions.FindAsync(request.SubscriptionId);
        if (subscription == null)
            throw new KeyNotFoundException($"ID {request.SubscriptionId} ile abonelik bulunamadı.");

        // Aynı dönem için başarılı ödeme var mı kontrol et
        var alreadyPaid = await _context.Payments
            .AnyAsync(p => p.SubscriptionId == request.SubscriptionId
                        && p.Period == request.Period
                        && p.Status == "Success");

        if (alreadyPaid)
            throw new InvalidOperationException($"Bu abonelik için {request.Period} döneminde zaten başarılı bir ödeme bulunmaktadır.");

        // Mock ödeme servisi çağrısı
        var externalResult = await _paymentExternal.ProcessPaymentAsync(
            request.Amount,
            subscription.SubscriberNumber,
            request.Period);

        var payment = new Payment
        {
            SubscriptionId = request.SubscriptionId,
            Amount = request.Amount,
            PaymentDateUtc = DateTime.UtcNow,
            Period = request.Period,
            Status = externalResult.IsSuccess ? "Success" : "Failed",
            ExternalTransactionId = externalResult.TransactionId,
            FailureReason = externalResult.FailureReason
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        return MapToDto(payment);
    }

    public async Task<List<PaymentDto>> GetBySubscriptionIdAsync(int subscriptionId)
    {
        var payments = await _context.Payments
            .AsNoTracking()
            .Where(p => p.SubscriptionId == subscriptionId)
            .OrderByDescending(p => p.PaymentDateUtc)
            .ToListAsync();

        return payments.Select(MapToDto).ToList();
    }

    public async Task<PaymentDto?> GetByIdAsync(int id)
    {
        var payment = await _context.Payments
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        return payment == null ? null : MapToDto(payment);
    }

    private static PaymentDto MapToDto(Payment payment)
    {
        return new PaymentDto
        {
            Id = payment.Id,
            SubscriptionId = payment.SubscriptionId,
            Amount = payment.Amount,
            PaymentDateUtc = payment.PaymentDateUtc,
            Period = payment.Period,
            Status = payment.Status,
            ExternalTransactionId = payment.ExternalTransactionId,
            FailureReason = payment.FailureReason
        };
    }
}
