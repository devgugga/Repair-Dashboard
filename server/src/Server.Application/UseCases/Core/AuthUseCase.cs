using AutoMapper;

using FluentValidation;
using FluentValidation.Results;

using Server.Application.DTOs.Request.Core;
using Server.Application.DTOs.Response.Core;
using Server.Application.UseCases.Interfaces.Core;
using Server.Domain.Entities.Core;
using Server.Domain.Exceptions.Security;
using Server.Domain.Interfaces.Repositories.Core;
using Server.Domain.Interfaces.Services.Core;
using Server.Domain.Interfaces.Services.Security;
using Server.Domain.ValueObjects.Params.Security;
using Server.Domain.ValueObjects.Results.Security;

namespace Server.Application.UseCases.Core;

public class AuthUseCase(
    IAuthService authService,
    IUserRepository userRepository,
    IRbacService rbacService,
    IMapper mapper,
    IValidator<AuthRequest> validator)
    : IAuthUseCase
{
    public async Task<AuthResponseWithToken> LoginAsync(AuthRequest request, string clientIp, string userAgent)
    {
        // 1. Validate request
        ValidationResult? validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);

        // 2. Map to domain params
        LoginCredentialsParams? credentials = mapper.Map<LoginCredentialsParams>(request);
        credentials.IpAddress = clientIp;
        credentials.UserAgent = userAgent;

        // 3. Call domain service
        AuthResult? authResult = await authService.LoginAsync(credentials);
        if (authResult == null)
            throw new InvalidCredentialsException();

        // 4. Map to wrapper DTO (includes refresh token for Controller)
        return mapper.Map<AuthResponseWithToken>(authResult);
    }

    public async Task<AuthResponseWithToken> RefreshTokenAsync(string? refreshToken, string clientIp, string userAgent)
    {
        // 1. Validate refresh token
        if (string.IsNullOrEmpty(refreshToken))
            throw new InvalidTokenException("Refresh token is required");

        // 2. Call domain service
        AuthResult? authResult = await authService.RefreshTokenAsync(refreshToken, clientIp, userAgent);
        if (authResult == null)
            throw new InvalidTokenException("Invalid or expired refresh token");

        // 3. Map to wrapper DTO (includes new refresh token for Controller)
        return mapper.Map<AuthResponseWithToken>(authResult);
    }

    public async Task<bool> LogoutAsync(string? refreshToken)
    {
        // 1. Validate refresh token
        if (string.IsNullOrEmpty(refreshToken))
            return false;

        // 2. Revoke refresh token
        return await authService.LogoutAsync(refreshToken);
    }

    public async Task<bool> LogoutAllAsync(Guid userId)
    {
        // 1. Revoke all refresh tokens for user
        return await authService.LogoutAllAsync(userId);
    }

    public async Task<AuthMeResponse> GetCurrentUserAsync(Guid userId)
    {
        // 1. Load current active user with person data
        User? user = await userRepository.GetWithPersonAsync(userId);
        if (user == null)
            throw new InvalidTokenException("Invalid user session");

        // 2. Load current role names and effective permissions
        IEnumerable<string> roleNames = await rbacService.GetUserRoleNamesAsync(userId);
        IEnumerable<string> permissions = await rbacService.GetUserPermissionNamesAsync(userId);

        // 3. Choose a primary role label for UI display
        string primaryRole = roleNames.OrderBy(role => role).FirstOrDefault() ?? string.Empty;

        // 4. Build profile response for /auth/me
        return new AuthMeResponse
        {
            User = new AuthUserResponse
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Person.Email,
                Role = primaryRole,
                LastLogin = user.LastLogin?.UtcDateTime ?? DateTime.UtcNow
            },
            Permissions = permissions.OrderBy(permission => permission).Distinct().ToArray()
        };
    }
}
