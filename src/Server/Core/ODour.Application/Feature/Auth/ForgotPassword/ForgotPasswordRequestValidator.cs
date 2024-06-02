using FastEndpoints;
using FluentValidation;
using ODour.Domain.Share.User.Entities;

namespace ODour.Application.Feature.Auth.ForgotPassword;

public sealed class ForgotPasswordRequestValidator : Validator<ForgotPasswordRequest>
{
    public ForgotPasswordRequestValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(expression: request => request.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(maximumLength: UserEntity.MetaData.Email.MaxLength)
            .MinimumLength(minimumLength: UserEntity.MetaData.Email.MinLength);
    }
}
