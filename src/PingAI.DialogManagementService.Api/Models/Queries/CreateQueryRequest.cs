using FluentValidation;
using PingAI.DialogManagementService.Api.Validations;

namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class CreateQueryRequest
    {
        public string Name { get; set; }
        public string ProjectId { get; set; }
        public string Description { get; set; }
        public string[]? Tags { get; set; }
        public int DisplayOrder { get; set; }
        public CreateIntentDto Intent { get; set; }
        public CreateResponseDto Response { get; set; }
    }

    public class CreateQueryRequestValidator : AbstractValidator<CreateQueryRequest>
    {
        public CreateQueryRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();
            RuleFor(x => x.ProjectId)
                .NotEmpty()
                .MustBeGuid();
            RuleFor(x => x.Description)
                .NotEmpty();
            RuleForEach(x => x.Tags).NotEmpty();
            RuleFor(x => x.Intent)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .SetValidator(new CreateIntentDtoValidator());
            RuleFor(x => x.Response)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .SetValidator(new CreateResponseDtoValidator());
        }
    }
}