using Microsoft.EntityFrameworkCore;
using SubscriptionReminder.Api.Models;
using BCrypt.Net;

namespace SubscriptionReminder.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // 1. Admin Kullanıcısı
        if (!await context.Users.AnyAsync(u => u.Email == "admin@subscription.com"))
        {
            var adminUser = new User
            {
                Email = "admin@subscription.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = "Admin",
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            };
            context.Users.Add(adminUser);
        }

        // 2. Müşteriler
        await SeedCustomerAsync(context, "Enes", "Yılmaz", "11111111111", "enes@example.com", "Enes123!");
        await SeedCustomerAsync(context, "Can", "Demir", "22222222222", "can@example.com", "Can123!");

        await context.SaveChangesAsync();
    }

    private static async Task SeedCustomerAsync(AppDbContext context, string first, string last, string tckn, string email, string password)
    {
        if (!await context.Customers.AnyAsync(c => c.Email == email || c.Tckn == tckn))
        {
            var customer = new Customer
            {
                FirstName = first,
                LastName = last,
                Tckn = tckn,
                Email = email,
                PhoneNumber = "5550000000",
                CreatedAtUtc = DateTime.UtcNow
            };
            context.Customers.Add(customer);
            await context.SaveChangesAsync(); // ID oluşması için

            var user = new User
            {
                CustomerId = customer.Id,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = "Customer",
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            };
            context.Users.Add(user);
        }
    }
}
