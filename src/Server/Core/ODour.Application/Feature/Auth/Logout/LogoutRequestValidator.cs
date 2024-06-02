using FastEndpoints;
using FluentValidation;

namespace ODour.Application.Feature.Auth.Logout;

public sealed class LogoutRequestValidator : Validator<LogoutRequest>
{
    public LogoutRequestValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(expression: request => request.RefreshToken).NotEmpty();
    }
}
