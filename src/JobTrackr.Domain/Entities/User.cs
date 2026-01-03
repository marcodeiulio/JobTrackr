using Microsoft.AspNetCore.Identity;

namespace JobTrackr.Domain.Entities;

public class User : IdentityUser<Guid> //already provides Id, UserName, Email, Password etc.
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}