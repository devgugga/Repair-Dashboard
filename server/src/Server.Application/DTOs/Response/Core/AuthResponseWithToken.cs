namespace Server.Application.DTOs.Response.Core;

public class AuthResponseWithToken : AuthResponse
{
    /// <summary>
    ///     Refresh token for Controller to set in HttpOnly cookie
    ///     (Not sent to client in JSON response)
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    ///     Refresh token expiration for cookie management
    /// </summary>
    public DateTime RefreshTokenExpiresAt { get; set; }
}
