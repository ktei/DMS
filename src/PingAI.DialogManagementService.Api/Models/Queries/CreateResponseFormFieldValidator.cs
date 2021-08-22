using FluentValidation;

namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class CreateResponseFormFieldValidator : AbstractValidator<CreateResponseFormDto.Field>
    {
        public CreateResponseFormFieldValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.DisplayName).NotEmpty();
        }
    }
}