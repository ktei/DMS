using FluentValidation;
using PingAI.DialogManagementService.Api.Validations;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Intents
{
    public class CreateIntentRequest
    {
        public string ProjectId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public CreatePhrasePartDto[]? PhraseParts { get; set; }
    }

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
            RuleForEach(x => x.PhraseParts)
                .SetValidator(new CreatePhrasePartDtoValidator());
        }
    }
}