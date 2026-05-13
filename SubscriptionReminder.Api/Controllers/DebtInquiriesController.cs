using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubscriptionReminder.Api.Services.Interfaces;

namespace SubscriptionReminder.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DebtInquiriesController : ControllerBase
{
    private readonly IDebtInquiryService _debtInquiryService;

    public DebtInquiriesController(IDebtInquiryService debtInquiryService)
    {
        _debtInquiryService = debtInquiryService;
    }

    /// <summary>
    /// Abonelik için borç sorgulama yapar (mock üçüncü parti servis).
    /// </summary>
    [HttpPost("{subscriptionId}/query")]
    public async Task<IActionResult> Query(int subscriptionId)
    {
        try
        {
            var result = await _debtInquiryService.QueryAsync(subscriptionId);
            return Ok(result);
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
    /// Aboneliğe ait borç sorgulama geçmişini listeler.
    /// </summary>
    [HttpGet("subscription/{subscriptionId}")]
    public async Task<IActionResult> GetBySubscriptionId(int subscriptionId)
    {
        var inquiries = await _debtInquiryService.GetBySubscriptionIdAsync(subscriptionId);
        return Ok(inquiries);
    }
}
