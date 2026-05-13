using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubscriptionReminder.Api.DTOs.Subscription;
using SubscriptionReminder.Api.Services.Interfaces;

namespace SubscriptionReminder.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionsController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    /// <summary>
    /// Yeni abonelik oluşturur.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSubscriptionRequest request)
    {
        var subscription = await _subscriptionService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = subscription.Id }, subscription);
    }

    /// <summary>
    /// Müşteriye ait tüm abonelikleri listeler.
    /// </summary>
    [HttpGet("customer/{customerId}")]
    public async Task<IActionResult> GetByCustomerId(int customerId)
    {
        var subscriptions = await _subscriptionService.GetAllByCustomerIdAsync(customerId);
        return Ok(subscriptions);
    }

    /// <summary>
    /// ID'ye göre abonelik getirir.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var subscription = await _subscriptionService.GetByIdAsync(id);
        if (subscription == null)
            return NotFound(new { message = $"ID {id} ile abonelik bulunamadı." });

        return Ok(subscription);
    }

    /// <summary>
    /// Abonelik bilgilerini günceller.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSubscriptionRequest request)
    {
        var subscription = await _subscriptionService.UpdateAsync(id, request);
        if (subscription == null)
            return NotFound(new { message = $"ID {id} ile abonelik bulunamadı." });

        return Ok(subscription);
    }

    /// <summary>
    /// Aboneliği siler.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _subscriptionService.DeleteAsync(id);
        if (!result)
            return NotFound(new { message = $"ID {id} ile abonelik bulunamadı." });

        return NoContent();
    }
}
