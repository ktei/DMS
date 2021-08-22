using FluentValidation;
using PingAI.DialogManagementService.Api.Validations;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class CreateResponseDtoValidator : AbstractValidator<CreateResponseDto>
    {
        public CreateResponseDtoValidator()
        {
            RuleFor(x => x.Type)
                .NotEmpty()
                .MustBeEnum(typeof(ResponseType));
            RuleFor(x => x.RteText)
                .NotEmpty()
                .MaximumLength(Response.MaxRteTextLength)
                .When(x => x.Type == ResponseType.RTE.ToString());
            RuleFor(x => x.RteText)
                .NotEmpty()
                .MaximumLength(Response.QuickReplyLength)
                .When(x => x.Type == ResponseType.QUICK_REPLY.ToString());
            RuleFor(x => x.Form)
                .NotNull()
                .When(x => x.Type == ResolutionType.FORM.ToString());
            RuleFor(x => x.Form!).SetValidator(new CreateResponseFormDtoValidator())
                .When(x => x.Form != null);
        }
    }
}