using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubscriptionReminder.Api.DTOs.DebtInquiry;
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

    [HttpPost("{id}/query")]
    public async Task<ActionResult<DebtInquiryDto>> Query(int id)
    {
        try
        {
            return Ok(await _debtInquiryService.QueryAsync(id));
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

    [HttpGet("{id}/status/{period}")]
    public async Task<ActionResult<DebtStatusDto>> GetStatus(int id, string period)
    {
        try
        {
            return Ok(await _debtInquiryService.GetStatusForPeriodAsync(id, period));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("subscription/{subscriptionId}")]
    public async Task<ActionResult<List<DebtInquiryDto>>> GetBySubscriptionId(int subscriptionId)
    {
        var inquiries = await _debtInquiryService.GetBySubscriptionIdAsync(subscriptionId);
        return Ok(inquiries);
    }
}
