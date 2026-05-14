using Microsoft.EntityFrameworkCore;
using SubscriptionReminder.Api.Data;
using SubscriptionReminder.Api.DTOs.Card;
using SubscriptionReminder.Api.Models;
using SubscriptionReminder.Api.Services.Interfaces;

namespace SubscriptionReminder.Api.Services;

public class SavedCardService : ISavedCardService
{
    private readonly AppDbContext _context;

    public SavedCardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SavedCardDto> CreateAsync(CreateSavedCardRequest request)
    {
        // Kart maskeleme işlemi: sadece son 4 hanesi
        var last4 = request.FullCardNumber.Substring(request.FullCardNumber.Length - 4);
        var masked = $"**** **** **** {last4}";

        var savedCard = new SavedCard
        {
            CustomerId = request.CustomerId,
            CardHolderName = request.CardHolderName,
            MaskedCardNumber = masked,
            ExpiryDate = request.ExpiryDate,
            CreatedAtUtc = DateTime.UtcNow
        };

        _context.SavedCards.Add(savedCard);
        await _context.SaveChangesAsync();

        return MapToDto(savedCard);
    }

    public async Task<List<SavedCardDto>> GetAllByCustomerIdAsync(int customerId)
    {
        var cards = await _context.SavedCards
            .AsNoTracking()
            .Where(c => c.CustomerId == customerId)
            .OrderByDescending(c => c.CreatedAtUtc)
            .ToListAsync();

        return cards.Select(MapToDto).ToList();
    }

    public async Task<bool> DeleteAsync(int id, int customerId)
    {
        var card = await _context.SavedCards.FirstOrDefaultAsync(c => c.Id == id && c.CustomerId == customerId);
        if (card == null) return false;

        _context.SavedCards.Remove(card);
        await _context.SaveChangesAsync();
        return true;
    }

    private static SavedCardDto MapToDto(SavedCard card)
    {
        return new SavedCardDto
        {
            Id = card.Id,
            CustomerId = card.CustomerId,
            CardHolderName = card.CardHolderName,
            MaskedCardNumber = card.MaskedCardNumber,
            ExpiryDate = card.ExpiryDate,
            CreatedAtUtc = card.CreatedAtUtc
        };
    }
}
