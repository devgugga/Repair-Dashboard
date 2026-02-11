using System.Security.Claims;

using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Server.Api.Extensions;
using Server.Application.DTOs.Request.Core;
using Server.Application.DTOs.Response.Core;
using Server.Application.UseCases.Interfaces.Core;

namespace Server.Api.Controllers.Core;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthUseCase authUseCase, IMapper mapper) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] AuthRequest request)
    {
        // UseCase returns wrapper with refresh token (exceptions handled by GlobalExceptionFilter)
        AuthResponseWithToken responseWithToken = await authUseCase.LoginAsync(
            request,
            HttpContext.GetClientIpAddress(), // Controller extracts
            GetUserAgent() // Controller extracts
        );

        // Controller responsibility: handle refresh token cookie
        SetRefreshTokenCookie(responseWithToken.RefreshToken, responseWithToken.RefreshTokenExpiresAt);

        // Convert wrapper to public response (without refresh token)
        AuthResponse publicResponse = mapper.Map<AuthResponse>(responseWithToken);

        return Ok(publicResponse);

    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh()
    {
        // Controller responsibility: extract refresh token from cookie
        string? refreshToken = GetRefreshTokenFromCookie();

        // UseCase handles validation and throws exceptions (handled by GlobalExceptionFilter)
        AuthResponseWithToken responseWithToken = await authUseCase.RefreshTokenAsync(
            refreshToken,
            HttpContext.GetClientIpAddress(),
            GetUserAgent()
        );

        // Controller responsibility: handle new refresh token cookie
        SetRefreshTokenCookie(responseWithToken.RefreshToken, responseWithToken.RefreshTokenExpiresAt);

        // Convert wrapper to public response (without refresh token)
        AuthResponse publicResponse = mapper.Map<AuthResponse>(responseWithToken);

        return Ok(publicResponse);

    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        // Controller responsibility: extract refresh token from cookie
        string? refreshToken = GetRefreshTokenFromCookie();
        bool success = await authUseCase.LogoutAsync(refreshToken);

        // Controller responsibility: clear cookie
        ClearRefreshTokenCookie();

        return Ok(new { success });
    }

    [HttpPost("logout-all")]
    [Authorize]
    public async Task<IActionResult> LogoutAll()
    {
        // Controller responsibility: extract user ID from claims
        Guid? userId = GetCurrentUserId();
        if (!userId.HasValue)
            return BadRequest("User not found");

        bool success = await authUseCase.LogoutAllAsync(userId.Value);

        // Controller responsibility: clear cookie
        ClearRefreshTokenCookie();

        return Ok(new { success });
    }

    [HttpPost("admin/logout-user/{userId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> LogoutUser(Guid userId)
    {
        bool success = await authUseCase.LogoutAllAsync(userId);
        return Ok(new { success });
    }

    #region Controller Helpers (ASP.NET Specific)

    private string GetUserAgent()
    {
        return Request.Headers.UserAgent.FirstOrDefault() ?? "Unknown";
    }

    private void SetRefreshTokenCookie(string refreshToken, DateTime expires)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // HTTPS only
            SameSite = SameSiteMode.Strict,
            Expires = expires
        };

        Response.Cookies.Append("server_refresh", refreshToken, cookieOptions);
    }

    private string? GetRefreshTokenFromCookie()
    {
        return Request.Cookies["server_refresh"];
    }

    private void ClearRefreshTokenCookie()
    {
        Response.Cookies.Delete("server_refresh");
    }

    private Guid? GetCurrentUserId()
    {
        string? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out Guid userId) ? userId : null;
    }

    #endregion
}
