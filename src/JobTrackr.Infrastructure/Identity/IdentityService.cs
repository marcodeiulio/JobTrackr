using JobTrackr.Application.Common.Interfaces;
using JobTrackr.Application.Common.Models;
using JobTrackr.Domain.Constants;
using JobTrackr.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace JobTrackr.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly IApplicationDbContext _context;
    private readonly JwtSettings _jwtSettings;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    public IdentityService(IApplicationDbContext context, IJwtTokenGenerator jwtTokenGenerator,
        IOptions<JwtSettings> jwtOptions,
        SignInManager<User> signInManager, UserManager<User> userManager)
    {
        _context = context;
        _jwtTokenGenerator = jwtTokenGenerator;
        _jwtSettings = jwtOptions.Value;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public async Task<Result<string>> RegisterAsync(string email, string password, string username,
        string? firstName = null,
        string? lastName = null)
    {
        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser is not null)
            return new Result<string>(
                false,
                null,
                "Email already in use.");

        var newUser = new User { Email = email, UserName = username, FirstName = firstName, LastName = lastName };

        var createResult = await _userManager.CreateAsync(newUser, password);
        if (!createResult.Succeeded)
        {
            var errors = string.Join(" ", createResult.Errors.Select(e => e.Description));
            return new Result<string>(false, null, errors);
        }

        await _userManager.AddToRoleAsync(newUser, Roles.User);
        await _context.SaveChangesAsync();

        return new Result<string>(true, newUser.Id.ToString(), string.Empty);
    }

    public async Task<Result<AuthResponse>> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return new Result<AuthResponse>(false, null, "Invalid email or password.");

        var loginResult = await _signInManager.CheckPasswordSignInAsync(user, password, true);

        if (loginResult.IsLockedOut)
            return new Result<AuthResponse>(false, null,
                "Account is locked due to multiple failed login attempts. Try again later.");
        if (loginResult.IsNotAllowed)
            return new Result<AuthResponse>(false, null, "Email not allowed. Try again later.");
        if (!loginResult.Succeeded)
            return new Result<AuthResponse>(false, null, "Invalid email or password.");

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtTokenGenerator.GenerateAccessToken(user, roles);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            CreatedAt = DateTime.UtcNow
        };

        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync();

        return new Result<AuthResponse>(true, new AuthResponse(accessToken, refreshToken),
            string.Empty);
    }
}