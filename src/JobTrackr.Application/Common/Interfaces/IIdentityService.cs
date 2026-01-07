using JobTrackr.Application.Common.Models;

namespace JobTrackr.Application.Common.Interfaces;

public record AuthResponse(string AccessToken, string RefreshToken);

public interface IIdentityService
{
    Task<Result<string>> RegisterAsync(
        string email,
        string password,
        string username,
        string? firstName = null,
        string? lastName = null
    );

    Task<Result<AuthResponse>> LoginAsync(string email, string password);
}