using System.Text.RegularExpressions;
using FluentValidation;
using PingAI.DialogManagementService.Api.Validations;

namespace PingAI.DialogManagementService.Api.Models.Projects
{
    public class UpdateProjectRequest
    {
        public string WidgetTitle { get; set; }
        public string WidgetColor { get; set; }
        public string WidgetDescription { get; set; }
        public string FallbackMessage { get; set; }
        public string GreetingMessage { get; set; }
        public string[]? Domains { get; set; }
        public string? BusinessTimeStart { get; set; }
        public string? BusinessTimeEnd { get; set; }
        public string? BusinessEmail { get; set; }
    }

    public class UpdateProjectRequestValidator : AbstractValidator<UpdateProjectRequest>
    {
        public UpdateProjectRequestValidator()
        {
            RuleFor(x => x.WidgetColor)
                .NotEmpty()
                .Must(x => string.IsNullOrEmpty(x) || 
                           Regex.IsMatch(x, @"^#(?:[0-9a-fA-F]{3}){1,2}$"))
                .WithMessage("{PropertyName} must be a valid HEX color code");
            RuleFor(x => x.FallbackMessage)
                .NotEmpty();

            RuleForEach(x => x.Domains)
                .NotEmpty()
                .MustBeUrl();

            RuleFor(x => x.BusinessTimeStart)
                .NotEmpty()
                .When(x => x.BusinessTimeEnd != null);
            RuleFor(x => x.BusinessTimeStart)
                .Must(x => ProjectDto.TryConvertStringToUtc(x!) != null)
                .When(x => x.BusinessTimeStart != null)
                .WithMessage("Business time start must be in a valid time format: yyyy-MM-ddTHH:mm:ss, " +
                             "e.g. 2020-09-19T15:23:11");

            RuleFor(x => x.BusinessTimeEnd)
                .NotEmpty()
                .When(x => x.BusinessTimeStart != null);
            RuleFor(x => x.BusinessTimeEnd)
                .Must(x => ProjectDto.TryConvertStringToUtc(x!) != null)
                .When(x => x.BusinessTimeEnd != null)
                .WithMessage("Business time end must be in a valid time format: yyyy-MM-ddTHH:mm:ss, " +
                             "e.g. 2020-09-19T15:23:11");

            RuleFor(x => x.BusinessEmail)
                .NotEmpty()
                .When(x => x.BusinessEmail != null);
        }
    }
}