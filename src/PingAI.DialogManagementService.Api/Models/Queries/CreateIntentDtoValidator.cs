using FluentValidation;
using PingAI.DialogManagementService.Api.Models.Intents;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class CreateIntentDtoValidator : AbstractValidator<CreateIntentDto>
    {
        public CreateIntentDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(Intent.MaxNameLength);

            RuleForEach(x => x.PhraseParts)
                .ForEach(x =>
                    x.SetValidator(new CreatePhrasePartDtoValidator()));

            RuleFor(x => x.Phrases)
                .ForEach(x => x.NotEmpty());
        }
    }
}