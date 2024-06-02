using FastEndpoints;
using FluentValidation;

namespace ODour.Application.Feature.Auth.RefreshAccessToken;

public sealed class RefreshAccessTokenRequestValidator : Validator<RefreshAccessTokenRequest>
{
    public RefreshAccessTokenRequestValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(expression: request => request.RefreshToken).NotEmpty();
    }
}
