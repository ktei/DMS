using FluentValidation;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class UpdateQueryDtoValidator : AbstractValidator<UpdateQueryDto>
    {
        public UpdateQueryDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(Query.MaxNameLength);
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