using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Server.Domain.Entities.Core;
using Server.Domain.Entities.Security;
using Server.Domain.Interfaces.Services.Security;
using Server.Domain.ValueObjects.Params.Security;
using Server.Domain.ValueObjects.Results.Security;
using Server.Infrastructure.Data;

namespace Server.Infrastructure.Services.Core;

public class AuthService(
    ServerDbContext context,
    IPasswordHashService passwordHashService,
    IJwtTokenService jwtTokenService,
    ILogger<AuthService> logger) : IAuthService
{
    public async Task<AuthResult?> LoginAsync(LoginCredentialsParams credentials)
    {
        try
        {
            // 1. Find user by username
            User? user = await context.Set<User>()
                .Include(u => u.Person)
                .FirstOrDefaultAsync(u => u.UserName == credentials.UserName);

            if (user == null)
            {
                logger.LogWarning("Login attempt failed: User {UserName} not found", credentials.UserName);
                return null;
            }

            // 2. Check if user is locked
            if (user.IsLocked)
            {
                logger.LogWarning("Login attempt failed: User {UserName} is locked", credentials.UserName);
                return null;
            }

            // 3. Verify password
            bool isPasswordValid =
                await passwordHashService.VerifyPasswordAsync(credentials.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                // Increment failed attempts
                user.FailedLoginAttempts++;

                // Lock user after 5 failed attempts
                if (user.FailedLoginAttempts >= 5)
                {
                    user.IsLocked = true;
                    logger.LogWarning("User {UserName} locked after {Attempts} failed attempts",
                        user.UserName, user.FailedLoginAttempts);
                }

                await context.SaveChangesAsync();
                logger.LogWarning("Login attempt failed: Invalid password for user {UserName}", credentials.UserName);
                return null;
            }

            // 4. Reset failed attempts on successful login
            user.FailedLoginAttempts = 0;
            user.LastLogin = DateTimeOffset.UtcNow;

            // 5. Generate tokens
            string accessToken = jwtTokenService.GenerateAccessToken(user);
            DateTime accessTokenExpiry = jwtTokenService.GetTokenExpiry(accessToken);
            RefreshToken refreshToken = GenerateRefreshToken(user.Id, credentials.IpAddress, credentials.UserAgent);

            // 6. Save refresh token
            context.Set<RefreshToken>().Add(refreshToken);
            await context.SaveChangesAsync();

            logger.LogInformation("User {UserName} logged in successfully", user.UserName);

            return AuthResult.Success(
                accessToken,
                refreshToken.Token,
                accessTokenExpiry,
                refreshToken.ExpiresAt,
                user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during login for user {UserName}", credentials.UserName);
            return null;
        }
    }

    public async Task<AuthResult?> RefreshTokenAsync(string refreshToken, string ipAddress, string userAgent)
    {
        try
        {
            // 1. Find and validate refresh token
            RefreshToken? storedToken = await context.Set<RefreshToken>()
                .Include(rt => rt.User)
                .ThenInclude(u => u.Person)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (storedToken is not { IsValid: true })
            {
                logger.LogWarning("Refresh token is invalid or expired");
                return null;
            }

            // 2. Revoke old refresh token
            storedToken.Revoke();

            // 3. Generate new tokens
            string accessToken = jwtTokenService.GenerateAccessToken(storedToken.User);
            DateTime accessTokenExpiry = jwtTokenService.GetTokenExpiry(accessToken);
            RefreshToken newRefreshToken = GenerateRefreshToken(storedToken.UserId, ipAddress, userAgent);

            // 4. Save new refresh token
            context.Set<RefreshToken>().Add(newRefreshToken);
            await context.SaveChangesAsync();

            logger.LogInformation("Tokens refreshed for user {UserId}", storedToken.UserId);

            return AuthResult.Success(
                accessToken,
                newRefreshToken.Token,
                accessTokenExpiry,
                newRefreshToken.ExpiresAt,
                storedToken.User);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during token refresh");
            return null;
        }
    }

    public async Task<bool> LogoutAsync(string refreshToken)
    {
        try
        {
            RefreshToken? storedToken = await context.Set<RefreshToken>()
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (storedToken == null || storedToken.IsRevoked)
                return false;

            storedToken.Revoke();
            await context.SaveChangesAsync();

            logger.LogInformation("User logged out successfully");
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during logout");
            return false;
        }
    }

    public async Task<bool> LogoutAllAsync(Guid userId)
    {
        try
        {
            List<RefreshToken> activeTokens = await context.Set<RefreshToken>()
                .Where(rt => rt.UserId == userId && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();

            foreach (RefreshToken token in activeTokens) token.Revoke();

            await context.SaveChangesAsync();

            logger.LogInformation("All sessions revoked for user {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during logout all for user {UserId}", userId);
            return false;
        }
    }

    public async Task<bool> IsRefreshTokenValidAsync(string refreshToken)
    {
        try
        {
            RefreshToken? storedToken = await context.Set<RefreshToken>()
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            return storedToken is { IsValid: true };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error validating refresh token");
            return false;
        }
    }

    public async Task<int> CleanExpiredTokensAsync()
    {
        try
        {
            List<RefreshToken> expiredTokens = await context.Set<RefreshToken>()
                .Where(rt => rt.IsExpired || rt.IsRevoked)
                .ToListAsync();

            context.Set<RefreshToken>().RemoveRange(expiredTokens);
            await context.SaveChangesAsync();

            logger.LogInformation("Cleaned {Count} expired/revoked tokens", expiredTokens.Count);
            return expiredTokens.Count;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error cleaning expired tokens");
            return 0;
        }
    }

    private static RefreshToken GenerateRefreshToken(Guid userId, string? ipAddress, string? userAgent)
    {
        return new RefreshToken
        {
            UserId = userId,
            Token = Guid.NewGuid().ToString().Replace("-", ""),
            ExpiresAt = DateTime.UtcNow.AddDays(7), // 7 days
            IpAddress = ipAddress,
            UserAgent = userAgent
        };
    }
}
