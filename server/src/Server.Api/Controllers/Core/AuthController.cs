using System.Security.Claims;

using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Server.Api.Attributes;
using Server.Api.Extensions;
using Server.Application.DTOs.Request.Core;
using Server.Application.DTOs.Response.Core;
using Server.Application.UseCases.Interfaces.Core;

namespace Server.Api.Controllers.Core;

/// <summary>
///     Handles authentication endpoints such as login, token refresh, and logout flows.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthUseCase authUseCase, IMapper mapper) : ControllerBase
{
    /// <summary>
    ///     Authenticates a user and returns an access token.
    /// </summary>
    /// <param name="request">The authentication request payload.</param>
    /// <returns>The authenticated user response with access token information.</returns>
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

    /// <summary>
    ///     Refreshes the access token using the refresh token cookie.
    /// </summary>
    /// <returns>The refreshed authentication response.</returns>
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

    /// <summary>
    ///     Logs out the current user by revoking the current refresh token.
    /// </summary>
    /// <returns>A success flag.</returns>
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

    /// <summary>
    ///     Logs out the current user from all sessions/devices.
    /// </summary>
    /// <returns>A success flag.</returns>
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

    /// <summary>
    ///     Administrative endpoint to revoke all sessions for a specific user.
    /// </summary>
    /// <param name="userId">The target user identifier.</param>
    /// <returns>A success flag.</returns>
    [HttpPost("admin/logout-user/{userId:guid}")]
    [RequirePermission("users", "manage_roles")]
    public async Task<IActionResult> LogoutUser(Guid userId)
    {
        bool success = await authUseCase.LogoutAllAsync(userId);
        return Ok(new { success });
    }

    /// <summary>
    ///     Returns the current authenticated user profile with effective permissions.
    /// </summary>
    /// <returns>The current user profile and permissions.</returns>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<AuthMeResponse>> Me()
    {
        // Controller responsibility: extract user ID from claims
        Guid? userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized();

        // UseCase handles profile and permission retrieval
        AuthMeResponse response = await authUseCase.GetCurrentUserAsync(userId.Value);
        return Ok(response);
    }

    #region Controller Helpers (ASP.NET Specific)

    /// <summary>
    ///     Gets the user-agent header value for the current request.
    /// </summary>
    /// <returns>The user-agent or <c>Unknown</c> when absent.</returns>
    private string GetUserAgent()
    {
        return Request.Headers.UserAgent.FirstOrDefault() ?? "Unknown";
    }

    /// <summary>
    ///     Writes the refresh token cookie in the response.
    /// </summary>
    /// <param name="refreshToken">The refresh token value.</param>
    /// <param name="expires">The cookie expiration date.</param>
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

    /// <summary>
    ///     Reads the refresh token from the request cookie.
    /// </summary>
    /// <returns>The refresh token, or <c>null</c> when absent.</returns>
    private string? GetRefreshTokenFromCookie()
    {
        return Request.Cookies["server_refresh"];
    }

    /// <summary>
    ///     Clears the refresh token cookie from the response.
    /// </summary>
    private void ClearRefreshTokenCookie()
    {
        Response.Cookies.Delete("server_refresh");
    }

    /// <summary>
    ///     Reads the current user id from claims.
    /// </summary>
    /// <returns>The current user id, or <c>null</c> when unavailable.</returns>
    private Guid? GetCurrentUserId()
    {
        string? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out Guid userId) ? userId : null;
    }

    #endregion
}
