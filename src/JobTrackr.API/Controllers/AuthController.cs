using JobTrackr.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JobTrackr.API.Controllers;

public record RegisterRequest(
    string Email,
    string Password,
    string UserName,
    string? FirstName,
    string? LastName
);

public record LoginRequest(
    string Email,
    string Password);

public record AuthResult(
    string AccessToken,
    string RefreshToken);

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IIdentityService _identityService;

    public AuthController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<string>> Register(RegisterRequest registerRequest)
    {
        var result = await _identityService.RegisterAsync(registerRequest.Email, registerRequest.Password,
            registerRequest.UserName, registerRequest.FirstName, registerRequest.LastName);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(new { userId = result.Data });
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LoginRequest loginRequest)
    {
        var result = await _identityService.LoginAsync(loginRequest.Email, loginRequest.Password);

        if (!result.IsSuccess)
            return Unauthorized(new { error = result.Error });

        return Ok(new AuthResult(result.Data!.AccessToken, result.Data.RefreshToken));
    }
}