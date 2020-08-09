using FluentValidation;

namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class CreateExpressionDto
    {
        public string EntityName { get; set; }
        public CreateExpressionDto Condition { get; set; }
    }
    
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