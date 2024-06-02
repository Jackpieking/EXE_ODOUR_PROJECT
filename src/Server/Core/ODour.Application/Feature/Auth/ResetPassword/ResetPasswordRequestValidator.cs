using FastEndpoints;
using FluentValidation;
using ODour.Domain.Share.User.Entities;

namespace ODour.Application.Feature.Auth.ResetPassword;

public sealed class ResetPasswordRequestValidator : Validator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(expression: request => request.NewPassword)
            .NotEmpty()
            .MaximumLength(maximumLength: UserEntity.MetaData.Password.MaxLength)
            .MinimumLength(minimumLength: UserEntity.MetaData.Password.MinLength);

        RuleFor(expression: request => request.ResetPasswordToken).NotEmpty();
    }
}
