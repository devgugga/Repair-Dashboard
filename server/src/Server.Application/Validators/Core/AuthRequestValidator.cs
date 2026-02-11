using FluentValidation;

using Server.Application.DTOs.Request.Core;

namespace Server.Application.Validators.Core;

internal class AuthRequestValidator : AbstractValidator<AuthRequest>
{
    public AuthRequestValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("Username is required")
            .WithErrorCode("USERNAME_REQUIRED");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .WithErrorCode("PASSWORD_REQUIRED");
    }

}
