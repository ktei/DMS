using FluentValidation;
using PingAI.DialogManagementService.Api.Validations;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class CreateQueryRequestV1_1
    {
        public string Name { get; set; }
        public string ProjectId { get; set; }
        public string? Description { get; set; }
        public string[]? Tags { get; set; }
        public int DisplayOrder { get; set; }
        public CreateIntentDto Intent { get; set; }
        public CreateResponseDto[] Responses { get; set; }
    }
    
    public class CreateQueryRequestV1_1Validator : AbstractValidator<CreateQueryRequestV1_1>
    {
        public CreateQueryRequestV1_1Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(Query.MaxNameLength);
            RuleFor(x => x.ProjectId)
                .NotEmpty()
                .MustBeGuid();
            RuleForEach(x => x.Tags).NotEmpty()
                .MaximumLength(Query.MaxTagLength);
            RuleFor(x => x.Intent)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .SetValidator(new CreateIntentDtoValidator());
            RuleForEach(x => x.Responses)
                .SetValidator(new CreateResponseDtoValidator());
        }
    }
}