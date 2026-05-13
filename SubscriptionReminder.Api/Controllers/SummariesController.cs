using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubscriptionReminder.Api.Services.Interfaces;
using System.Security.Claims;

namespace SubscriptionReminder.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SummariesController : ControllerBase
{
    private readonly ISummaryService _summaryService;

    public SummariesController(ISummaryService summaryService)
    {
        _summaryService = summaryService;
    }

    private int GetCustomerId()
    {
        var customerIdClaim = User.FindFirst("CustomerId")?.Value;
        if (string.IsNullOrEmpty(customerIdClaim))
            throw new UnauthorizedAccessException("Customer ID not found in token.");

        return int.Parse(customerIdClaim);
    }

    /// <summary>
    /// Kullanıcının genel dashboard özetini getirir.
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboardSummary()
    {
        try
        {
            var customerId = GetCustomerId();
            var summary = await _summaryService.GetDashboardSummaryAsync(customerId);
            return Ok(summary);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Bu ay ödemesi yapılmamış aktif abonelikleri getirir.
    /// </summary>
    [HttpGet("unpaid")]
    public async Task<IActionResult> GetUnpaidSubscriptions()
    {
        try
        {
            var customerId = GetCustomerId();
            var unpaid = await _summaryService.GetUnpaidSubscriptionsAsync(customerId);
            return Ok(unpaid);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Kullanıcının tüm ödeme geçmişini getirir.
    /// </summary>
    [HttpGet("payments")]
    public async Task<IActionResult> GetPaymentHistory()
    {
        try
        {
            var customerId = GetCustomerId();
            var history = await _summaryService.GetPaymentHistoryAsync(customerId);
            return Ok(history);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}
