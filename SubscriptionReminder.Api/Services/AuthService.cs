using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SubscriptionReminder.Api.Data;
using SubscriptionReminder.Api.DTOs.Auth;
using SubscriptionReminder.Api.Models;
using SubscriptionReminder.Api.Services.Interfaces;

namespace SubscriptionReminder.Api.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Email daha önce kayıtlı mı?
        var existingUser = await _context.Users.AnyAsync(u => u.Email == request.Email);
        if (existingUser)
            throw new InvalidOperationException("Bu email adresi zaten kayıtlı.");

        // TCKN daha önce kayıtlı mı?
        var existingCustomer = await _context.Customers.AnyAsync(c => c.Tckn == request.Tckn);
        if (existingCustomer)
            throw new InvalidOperationException("Bu TCKN zaten kayıtlı.");

        // Müşteri oluştur
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

        // User oluştur
        var user = new User
        {
            CustomerId = customer.Id,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = "Customer",
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = GenerateJwtToken(user);

        return new AuthResponse
        {
            Token = token,
            Email = user.Email,
            Role = user.Role,
            UserId = user.Id,
            CustomerId = user.CustomerId
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null)
            throw new UnauthorizedAccessException("Email veya şifre hatalı.");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("Hesabınız devre dışı bırakılmış.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Email veya şifre hatalı.");

        var token = GenerateJwtToken(user);

        return new AuthResponse
        {
            Token = token,
            Email = user.Email,
            Role = user.Role,
            UserId = user.Id,
            CustomerId = user.CustomerId
        };
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role)
        };

        if (user.CustomerId.HasValue)
            claims.Add(new Claim("CustomerId", user.CustomerId.Value.ToString()));

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
