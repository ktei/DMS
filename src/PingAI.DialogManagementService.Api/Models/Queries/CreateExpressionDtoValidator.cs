using FluentValidation;

namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class CreateExpressionDtoValidator : AbstractValidator<CreateExpressionDto>
    {
        public CreateExpressionDtoValidator()
        {
            RuleFor(x => x.EntityName)
                .NotEmpty();

            RuleFor(x => x.Condition)
                .NotNull();
        }
    }
}