using FluentValidation;

namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class CreateResponseFormDtoValidator : AbstractValidator<CreateResponseFormDto>
    {
        public CreateResponseFormDtoValidator()
        {
            RuleForEach(x => x.Fields).SetValidator(new CreateResponseFormFieldValidator());
        }
    }
}