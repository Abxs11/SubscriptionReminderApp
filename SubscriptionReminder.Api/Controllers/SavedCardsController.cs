using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubscriptionReminder.Api.DTOs.Card;
using SubscriptionReminder.Api.Services.Interfaces;

namespace SubscriptionReminder.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SavedCardsController : ControllerBase
{
    private readonly ISavedCardService _savedCardService;

    public SavedCardsController(ISavedCardService savedCardService)
    {
        _savedCardService = savedCardService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSavedCardRequest request)
    {
        var customerIdClaim = User.FindFirst("CustomerId")?.Value;
        if (string.IsNullOrEmpty(customerIdClaim))
            return Unauthorized(new { message = "Müşteri bilgisi token içerisinde bulunamadı." });

        request.CustomerId = int.Parse(customerIdClaim); // Force customer ID to be the logged in user
        
        var savedCard = await _savedCardService.CreateAsync(request);
        return Ok(savedCard);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllMyCards()
    {
        var customerIdClaim = User.FindFirst("CustomerId")?.Value;
        if (string.IsNullOrEmpty(customerIdClaim))
            return Unauthorized(new { message = "Müşteri bilgisi token içerisinde bulunamadı." });

        int customerId = int.Parse(customerIdClaim);
        var cards = await _savedCardService.GetAllByCustomerIdAsync(customerId);
        
        return Ok(cards);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var customerIdClaim = User.FindFirst("CustomerId")?.Value;
        if (string.IsNullOrEmpty(customerIdClaim))
            return Unauthorized(new { message = "Müşteri bilgisi token içerisinde bulunamadı." });

        int customerId = int.Parse(customerIdClaim);
        var result = await _savedCardService.DeleteAsync(id, customerId);
        
        if (!result)
            return NotFound(new { message = $"ID {id} ile kart bulunamadı veya yetkiniz yok." });

        return NoContent();
    }
}
