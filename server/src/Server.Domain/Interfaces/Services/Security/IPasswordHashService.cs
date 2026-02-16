namespace Server.Domain.Interfaces.Services.Security;

public interface IPasswordHashService
{
    /// <summary>
    ///     Generates a password hash using Argon2id.
    /// </summary>
    /// <param name="password">The plain-text password.</param>
    /// <returns>The password hash including salt and parameters.</returns>
    Task<string> HashPasswordAsync(string password);

    /// <summary>
    ///     Verifies whether a password matches a stored hash.
    /// </summary>
    /// <param name="password">The plain-text password.</param>
    /// <param name="hash">The stored password hash.</param>
    /// <returns><c>true</c> when the password is valid; otherwise <c>false</c>.</returns>
    Task<bool> VerifyPasswordAsync(string password, string hash);

    /// <summary>
    ///     Checks whether a stored hash should be regenerated due to parameter changes.
    /// </summary>
    /// <param name="hash">The current hash value.</param>
    /// <returns><c>true</c> when rehashing is required; otherwise <c>false</c>.</returns>
    bool NeedsRehash(string hash);
}
