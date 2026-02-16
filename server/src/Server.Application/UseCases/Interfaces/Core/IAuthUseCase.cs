using Server.Application.DTOs.Request.Core;
using Server.Application.DTOs.Response.Core;

namespace Server.Application.UseCases.Interfaces.Core;

public interface IAuthUseCase
{
    /// <summary>
    ///     Authenticate user and generate tokens
    ///     Returns wrapper with refresh token for Controller cookie management
    /// </summary>
    Task<AuthResponseWithToken> LoginAsync(AuthRequest request, string clientIp, string userAgent);

    /// <summary>
    ///     Refresh access token using refresh token
    ///     Returns wrapper with new refresh token for Controller cookie management
    /// </summary>
    Task<AuthResponseWithToken> RefreshTokenAsync(string? refreshToken, string clientIp, string userAgent);

    /// <summary>
    ///     Logout current user (revoke refresh token)
    /// </summary>
    Task<bool> LogoutAsync(string? refreshToken);

    /// <summary>
    ///     Logout from all devices (revoke all refresh tokens)
    /// </summary>
    Task<bool> LogoutAllAsync(Guid userId);

    /// <summary>
    ///     Gets the current authenticated user profile and effective permissions.
    /// </summary>
    Task<AuthMeResponse> GetCurrentUserAsync(Guid userId);
}
