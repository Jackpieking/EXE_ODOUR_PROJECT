using FastEndpoints;
using FluentValidation;

namespace ODour.Application.Feature.User.Product.GetRelatedProductsByCategoryId;

public sealed class GetRelatedProductsByCategoryIdRequestValidator
    : Validator<GetRelatedProductsByCategoryIdRequest>
{
    public GetRelatedProductsByCategoryIdRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(expression: request => request.CategoryId)
            .NotEmpty()
            .Must(predicate: categoryId =>
                categoryId != Share.Common.CommonConstant.App.DefaultGuidValue
            );
    }
}
