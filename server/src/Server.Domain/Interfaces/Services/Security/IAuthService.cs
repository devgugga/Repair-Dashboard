using Server.Domain.ValueObjects.Params.Security;
using Server.Domain.ValueObjects.Results.Security;

namespace Server.Domain.Interfaces.Services.Security;

public interface IAuthService
{
    /// <summary>
    ///     Authenticate user and generate tokens
    /// </summary>
    Task<AuthResult?> LoginAsync(LoginCredentialsParams credentials);

    /// <summary>
    ///     Refresh access token using refresh token
    /// </summary>
    Task<AuthResult?> RefreshTokenAsync(string refreshToken, string ipAddress, string userAgent);

    /// <summary>
    ///     Logout user and revoke refresh token
    /// </summary>
    Task<bool> LogoutAsync(string refreshToken);

    /// <summary>
    ///     Logout from all devices (revoke all refresh tokens)
    /// </summary>
    Task<bool> LogoutAllAsync(Guid userId);

    /// <summary>
    ///     Validate if refresh token is active
    /// </summary>
    Task<bool> IsRefreshTokenValidAsync(string refreshToken);

    /// <summary>
    ///     Clean expired refresh tokens
    /// </summary>
    Task<int> CleanExpiredTokensAsync();
}
