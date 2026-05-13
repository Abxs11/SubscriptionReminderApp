using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubscriptionReminder.Api.DTOs.Customer;
using SubscriptionReminder.Api.Services.Interfaces;
using System.Security.Claims;

namespace SubscriptionReminder.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    /// <summary>
    /// Yeni müşteri oluşturur (Sadece Admin).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request)
    {
        var customer = await _customerService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
    }

    /// <summary>
    /// Tüm müşterileri listeler (Sadece Admin).
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var customers = await _customerService.GetAllAsync();
        return Ok(customers);
    }

    /// <summary>
    /// ID'ye göre müşteri getirir.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var customer = await _customerService.GetByIdAsync(id);
        if (customer == null)
            return NotFound(new { message = $"ID {id} ile müşteri bulunamadı." });

        return Ok(customer);
    }

    /// <summary>
    /// Giriş yapmış olan kullanıcının profil bilgilerini getirir.
    /// </summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var customerIdClaim = User.FindFirst("CustomerId")?.Value;
        if (string.IsNullOrEmpty(customerIdClaim))
            return Unauthorized(new { message = "Müşteri bilgisi token içerisinde bulunamadı." });

        int customerId = int.Parse(customerIdClaim);
        var customer = await _customerService.GetByIdAsync(customerId);
        
        if (customer == null)
            return NotFound(new { message = "Müşteri bulunamadı." });

        return Ok(customer);
    }

    /// <summary>
    /// Müşterinin kendi hesabını silmesini sağlar.
    /// </summary>
    [HttpDelete("me")]
    public async Task<IActionResult> DeleteMe()
    {
        var customerIdClaim = User.FindFirst("CustomerId")?.Value;
        if (string.IsNullOrEmpty(customerIdClaim))
            return Unauthorized(new { message = "Müşteri bilgisi token içerisinde bulunamadı." });

        int customerId = int.Parse(customerIdClaim);
        var result = await _customerService.DeleteAsync(customerId);
        
        if (!result)
            return NotFound(new { message = "Hesap bulunamadı." });

        return NoContent();
    }

    /// <summary>
    /// Belirli bir müşteriyi siler (Sadece Admin).
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _customerService.DeleteAsync(id);
        if (!result)
            return NotFound(new { message = $"ID {id} ile müşteri bulunamadı." });

        return NoContent();
    }
}
