using System;
using FluentValidation;
using PingAI.DialogManagementService.Api.Validations;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Responses
{
    public class CreateResponseRequestValidator : AbstractValidator<CreateResponseRequest>
    {
        public CreateResponseRequestValidator()
        {
            RuleFor(x => x.ProjectId)
                .NotEmpty()
                .MustBeGuid();
            RuleFor(x => x.Type)
                .NotEmpty()
                .MustBeEnum(typeof(ResponseType));
            RuleFor(x => x.RteText)
                .NotNull()
                .When(x => 
                    string.Compare(x.Type, ResponseType.RTE.ToString(), StringComparison.OrdinalIgnoreCase) == 0);
        }
    }
}