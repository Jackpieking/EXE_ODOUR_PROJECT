using FastEndpoints;
using FluentValidation;

namespace ODour.Application.Feature.Auth.ConfirmUserEmail;

public sealed class ConfirmUserEmailRequestValidator : Validator<ConfirmUserEmailRequest>
{
    public ConfirmUserEmailRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(expression: request => request.Token).NotEmpty();
    }
}
