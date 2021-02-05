using FluentValidation;
using PingAI.DialogManagementService.Api.Validations;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class CreateResponseDto
    {
        public string Type { get; set; }
        public string? RteText { get; set; }
        public CreateResponseFormDto? Form { get; set; }
        public int Order { get; set; }

    }

    public class CreateResponseFormDto
    {
        public Field[] Fields { get; set; }
        
        public class Field
        {
            public string Name { get; set; }
            public string DisplayName { get; set; } 
        }
    }

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

    public class CreateResponseFormDtoValidator : AbstractValidator<CreateResponseFormDto>
    {
        public CreateResponseFormDtoValidator()
        {
            RuleForEach(x => x.Fields).SetValidator(new CreateResponseFormFieldValidator());
        }
    }

    public class CreateResponseFormFieldValidator : AbstractValidator<CreateResponseFormDto.Field>
    {
        public CreateResponseFormFieldValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.DisplayName).NotEmpty();
        }
    }
}