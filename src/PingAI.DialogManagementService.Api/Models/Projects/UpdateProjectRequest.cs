using System.Text.RegularExpressions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
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
        public string[] Enquiries { get; set; }
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
            RuleFor(x => x.GreetingMessage)
                .NotEmpty();
        }
    }
}