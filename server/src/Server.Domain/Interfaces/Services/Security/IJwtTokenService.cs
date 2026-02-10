using System.Security.Claims;

using Server.Domain.Entities.Core;

namespace Server.Domain.Interfaces.Services.Security;

public interface IJwtTokenService
{
    /// <summary>
    ///     Generate access token for user
    /// </summary>
    string GenerateAccessToken(User user);

    /// <summary>
    ///     Generate refresh token
    /// </summary>
    string GenerateRefreshToken();

    /// <summary>
    ///     Get claims from access token
    /// </summary>
    ClaimsPrincipal? GetClaimsFromToken(string token);

    /// <summary>
    ///     Validate access token format and signature
    /// </summary>
    bool ValidateAccessToken(string token);

    /// <summary>
    ///     Get user ID from token claims
    /// </summary>
    Guid? GetUserIdFromToken(string token);

    /// <summary>
    ///     Get token expiry time
    /// </summary>
    DateTime GetTokenExpiry(string token);
}
