using FastEndpoints;
using FluentValidation;
using ODour.Domain.Share.User.Entities;

namespace ODour.Application.Feature.Auth.ResendUserConfirmationEmail;

public sealed class ResendUserConfirmationEmailRequestValidator
    : Validator<ResendUserConfirmationEmailRequest>
{
    public ResendUserConfirmationEmailRequestValidator()
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
