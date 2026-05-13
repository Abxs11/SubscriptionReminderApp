using SubscriptionReminder.Api.DTOs.Auth;

namespace SubscriptionReminder.Api.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
}
