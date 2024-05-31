using System.Linq;
using FastEndpoints;
using FluentValidation;
using ODour.Domain.Share.System.Entities;

namespace ODour.Application.Feature.Auth.RegisterAsAdmin;

public sealed class RegisterAsAdminRequestValidator : Validator<RegisterAsAdminRequest>
{
    public RegisterAsAdminRequestValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(expression: request => request.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(maximumLength: SystemAccountEntity.MetaData.Email.MaxLength)
            .MinimumLength(minimumLength: SystemAccountEntity.MetaData.Email.MinLength);

        RuleFor(expression: request => request.Password)
            .NotEmpty()
            .Must(predicate: password =>
            {
                if (
                    password.Length < 4
                    || !password.Any(predicate: char.IsUpper)
                    || !password.Any(predicate: char.IsLower)
                    || !password.Any(predicate: char.IsDigit)
                    || !password.Any(predicate: letter =>
                        "!@#$%^&*()-_=+{};:',.<>?".Contains(value: letter)
                    )
                )
                {
                    return false;
                }

                return true;
            })
            .MaximumLength(maximumLength: SystemAccountEntity.MetaData.Password.MaxLength)
            .MinimumLength(minimumLength: SystemAccountEntity.MetaData.Password.MinLength);

        RuleFor(expression: request => request.AdminConfirmedKey).NotEmpty();
    }
}
