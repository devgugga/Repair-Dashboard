using AutoMapper;

using FluentValidation;
using FluentValidation.Results;

using Server.Application.DTOs.Request.Core;
using Server.Application.DTOs.Response.Core;
using Server.Application.UseCases.Interfaces.Core;
using Server.Domain.Exceptions.Security;
using Server.Domain.Interfaces.Services.Security;
using Server.Domain.ValueObjects.Params.Security;
using Server.Domain.ValueObjects.Results.Security;

namespace Server.Application.UseCases.Core;

public class AuthUseCase(
    IAuthService authService,
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
}
