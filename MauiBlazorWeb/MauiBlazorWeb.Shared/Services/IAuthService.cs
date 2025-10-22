namespace MauiBlazorWeb.Shared.Services;

public interface IAuthService
{
    Task<bool> LoginAsync(string email, string password);
    Task<bool> LogoutAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<bool> EnsureAuthenticatedAsync();
    Task<string?> GetCurrentUserIdAsync();
    Task<string[]?> GetCurrentUserRolesAsync();
    Task<bool> IsInRoleAsync(string role);
    bool IsInRoleSync(string role); // For synchronous checks in UI bindings
}