using Microsoft.EntityFrameworkCore;
using SubscriptionReminder.Api.Data;
using SubscriptionReminder.Api.DTOs.Customer;
using SubscriptionReminder.Api.Models;
using SubscriptionReminder.Api.Services.Interfaces;

namespace SubscriptionReminder.Api.Services;

public class CustomerService : ICustomerService
{
    private readonly AppDbContext _context;

    public CustomerService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerRequest request)
    {
        var customer = new Customer
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Tckn = request.Tckn,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            CreatedAtUtc = DateTime.UtcNow
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return MapToDto(customer);
    }

    public async Task<List<CustomerDto>> GetAllAsync()
    {
        var customers = await _context.Customers
            .AsNoTracking()
            .OrderByDescending(c => c.CreatedAtUtc)
            .ToListAsync();

        return customers.Select(MapToDto).ToList();
    }

    public async Task<CustomerDto?> GetByIdAsync(int id)
    {
        var customer = await _context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        return customer == null ? null : MapToDto(customer);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null)
            return false;

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
        return true;
    }

    private static CustomerDto MapToDto(Customer customer)
    {
        return new CustomerDto
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Tckn = customer.Tckn,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber,
            CreatedAtUtc = customer.CreatedAtUtc
        };
    }
}
