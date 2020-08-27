using FluentValidation;
using PingAI.DialogManagementService.Api.Models.Intents;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class CreateIntentDto
    {
        public string Name { get; set; }
        public CreatePhrasePartDto[][] PhraseParts { get; set; }
    }

    public class CreateIntentDtoValidator : AbstractValidator<CreateIntentDto>
    {
        public CreateIntentDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(Intent.MaxNameLength);

            RuleForEach(x => x.PhraseParts)
                .ForEach(x => x.SetValidator(new CreatePhrasePartDtoValidator()));
        }
    }
}