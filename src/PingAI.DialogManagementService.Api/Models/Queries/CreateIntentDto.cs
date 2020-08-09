using FluentValidation;
using PingAI.DialogManagementService.Api.Models.Intents;

namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class CreateIntentDto
    {
        public string Name { get; set; }
        public CreatePhrasePartDto[] PhraseParts { get; set; }
    }

    public class CreateIntentDtoValidator : AbstractValidator<CreateIntentDto>
    {
        public CreateIntentDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();

            RuleForEach(x => x.PhraseParts).SetValidator(new CreatePhrasePartDtoValidator());
        }
    }
}