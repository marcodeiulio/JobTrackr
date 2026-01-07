using System.ComponentModel.DataAnnotations;

namespace JobTrackr.Infrastructure.Identity;

public record JwtSettings
{
    [Required] [MinLength(32)] public string Key { get; init; } = string.Empty;

    [Required] public string Issuer { get; init; } = string.Empty;

    [Required] public string Audience { get; init; } = string.Empty;

    [Range(1, 1440)] public int AccessTokenExpirationMinutes { get; init; }

    [Range(1, 60)] public int RefreshTokenExpirationDays { get; init; }
}