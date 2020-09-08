using System;
using FluentValidation;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class UpdateQueryRequest
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string[]? Tags { get; set; }
        public int DisplayOrder { get; set; }
        public CreateIntentDto Intent { get; set; }
        public CreateResponseDto Response { get; set; }
    }

    public class UpdateQueryRequestValidator : AbstractValidator<UpdateQueryRequest>
    {
        public UpdateQueryRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(Query.MaxNameLength);
            RuleForEach(x => x.Tags).NotEmpty()
                .MaximumLength(Query.MaxTagLength);
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