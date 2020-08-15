using FluentValidation;

namespace PingAI.DialogManagementService.Api.Models.Intents
{
    public class UpdateIntentRequest
    {
        public string Name { get; set; }
        public CreatePhrasePartDto[][]? Phrases { get; set; }
    }

    public class UpdateIntentRequestValidator : AbstractValidator<UpdateIntentRequest>
    {
        public UpdateIntentRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(255);
            RuleForEach(x => x.Phrases)
                .ForEach(x => x.SetValidator(new CreatePhrasePartDtoValidator()));
        }
    }
}