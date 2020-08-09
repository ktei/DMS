using System;
using FluentValidation;
using PingAI.DialogManagementService.Api.Validations;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class CreateConditionDto
    {
        public string Operator { get; set; }
        public string? Value { get; set; }
        public string? EntityNameId { get; set; }
        public string Comparer { get; set; } 
    }

    public class CreateConditionDtoValidator : AbstractValidator<CreateConditionDto>
    {
        public CreateConditionDtoValidator()
        {
            RuleFor(x => x.Operator)
                .NotEmpty()
                .MustBeEnum(typeof(Operator));
            RuleFor(x => x.Comparer)
                .NotEmpty()
                .MustBeEnum(typeof(Comparer));
            RuleFor(x => x.Value)
                .NotNull()
                .When(x => 
                    string.Compare(x.Comparer, Comparer.VALUE.ToString(), StringComparison.OrdinalIgnoreCase) == 0)
                .UseJsonPathInErrorMessage()
                .WithMessage("'{PropertyName}' must not be null when Comparer is VALUE");
            RuleFor(x => x.EntityNameId)
                .NotEmpty()!
                .MustBeGuid()
                .When(x => 
                    string.Compare(x.Comparer, Comparer.OTHER_ENTITY_NAME.ToString(), StringComparison.OrdinalIgnoreCase) == 0)
                .UseJsonPathInErrorMessage()
                .WithMessage("'{PropertyName}' must not be null when Comparer is OTHER_ENTITY_NAME");
        }
    }
}