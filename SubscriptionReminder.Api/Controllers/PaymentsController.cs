using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubscriptionReminder.Api.DTOs.Payment;
using SubscriptionReminder.Api.Services.Interfaces;

namespace SubscriptionReminder.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    /// <summary>
    /// Ödeme gerçekleştirir (mock ödeme servisi kullanılır).
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePaymentRequest request)
    {
        try
        {
            var payment = await _paymentService.CreateAsync(request);
            
            if (payment.Status == "Failed")
            {
                return BadRequest(new { 
                    message = "Ödeme işlemi banka tarafından reddedildi.", 
                    reason = payment.FailureReason,
                    paymentId = payment.Id 
                });
            }

            return CreatedAtAction(nameof(GetById), new { id = payment.Id }, payment);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Aboneliğe ait ödeme geçmişini listeler.
    /// </summary>
    [HttpGet("subscription/{subscriptionId}")]
    public async Task<IActionResult> GetBySubscriptionId(int subscriptionId)
    {
        var payments = await _paymentService.GetBySubscriptionIdAsync(subscriptionId);
        return Ok(payments);
    }

    /// <summary>
    /// ID'ye göre ödeme getirir.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var payment = await _paymentService.GetByIdAsync(id);
        if (payment == null)
            return NotFound(new { message = $"ID {id} ile ödeme bulunamadı." });

        return Ok(payment);
    }
}
