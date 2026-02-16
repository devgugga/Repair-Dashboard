namespace Server.Application.DTOs.Response.Core;

/// <summary>
///     Current authenticated user profile with effective permissions.
/// </summary>
public class AuthMeResponse
{
    /// <summary>
    ///     Gets or sets current user information.
    /// </summary>
    public AuthUserResponse User { get; set; } = new();

    /// <summary>
    ///     Gets or sets effective permissions for the current user.
    /// </summary>
    public string[] Permissions { get; set; } = [];
}
