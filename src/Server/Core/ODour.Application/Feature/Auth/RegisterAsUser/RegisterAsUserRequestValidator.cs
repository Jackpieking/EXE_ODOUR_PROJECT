using FastEndpoints;
using FluentValidation;
using ODour.Domain.Share.User.Entities;

namespace ODour.Application.Feature.Auth.RegisterAsUser;

public sealed class RegisterAsUserRequestValidator : Validator<RegisterAsUserRequest>
{
    public RegisterAsUserRequestValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(expression: request => request.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(maximumLength: UserEntity.MetaData.Email.MaxLength)
            .MinimumLength(minimumLength: UserEntity.MetaData.Email.MinLength);

        RuleFor(expression: request => request.Password)
            .NotEmpty()
            .MaximumLength(maximumLength: UserEntity.MetaData.Password.MaxLength)
            .MinimumLength(minimumLength: UserEntity.MetaData.Password.MinLength);
    }
}
