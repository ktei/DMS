using FluentValidation;
using PingAI.DialogManagementService.Api.Validations;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class CreateQueryDtoValidator : AbstractValidator<CreateQueryDto>
    {
        public CreateQueryDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(Query.MaxNameLength);
            RuleFor(x => x.ProjectId)
                .NotEmpty()
                .MustBeGuid();
            RuleForEach(x => x.Tags).NotEmpty()
                .MaximumLength(Query.MaxTagLength);
            RuleFor(x => x.Intent)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .SetValidator(new CreateIntentDtoValidator());
            RuleFor(x => x.Responses)
                .NotNull();
            RuleForEach(x => x.Responses)
                .SetValidator(new CreateResponseDtoValidator());
        }
    }
}