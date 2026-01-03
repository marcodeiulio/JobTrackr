using JobTrackr.Domain.Entities;

namespace JobTrackr.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(User user, IList<string> roles);
    string GenerateRefreshToken();
}