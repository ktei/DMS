using FluentValidation;
using PingAI.DialogManagementService.Api.Validations;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class CreateResponseDto
    {
        public string Type { get; set; }
        public string RteText { get; set; }
        public int Order { get; set; }
    }

    public class CreateResponseDtoValidator : AbstractValidator<CreateResponseDto>
    {
        public CreateResponseDtoValidator()
        {
            RuleFor(x => x.Type)
                .NotEmpty()
                .MustBeEnum(typeof(ResponseType));
            RuleFor(x => x.RteText)
                .NotEmpty();
        }
    }
}