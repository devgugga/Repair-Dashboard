namespace Server.Application.DTOs.Request.Core;

public class AuthRequest
{
    public required string UserName { get; set; }

    public required string Password { get; set; }

    /// <summary>
    ///     Remember me for longer refresh token expiration
    /// </summary>
    public bool RememberMe { get; set; } = false;
}
