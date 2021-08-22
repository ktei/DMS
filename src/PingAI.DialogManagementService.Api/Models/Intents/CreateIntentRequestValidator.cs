using FluentValidation;
using PingAI.DialogManagementService.Api.Validations;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Intents
{
    public class CreateIntentRequestValidator : AbstractValidator<CreateIntentRequest>
    {
        public CreateIntentRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(255);
            RuleFor(x => x.ProjectId)
                .NotEmpty()
                .MustBeGuid();
            RuleFor(x => x.Type)
                .NotEmpty()
                .MustBeEnum(typeof(IntentType));
            RuleForEach(x => x.Phrases)
                .ForEach(x => x.SetValidator(new CreatePhrasePartDtoValidator()));
        }
    }
}