using System;
using FluentValidation;
using PingAI.DialogManagementService.Api.Validations;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Responses
{
    public class CreateResponseRequest
    {
        public string ProjectId { get; set; }
        public string Type { get; set; }
        public string? RteText { get; set; }
        public int Order { get; set; }
    }

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