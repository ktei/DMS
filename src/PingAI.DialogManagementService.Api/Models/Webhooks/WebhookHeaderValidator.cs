using System.Collections.Generic;
using FluentValidation;

namespace PingAI.DialogManagementService.Api.Models.Webhooks
{
    public class WebhookHeaderValidator : AbstractValidator<KeyValuePair<string, string>>
    {
        public WebhookHeaderValidator()
        {
            RuleFor(x => x.Key).NotEmpty();
            RuleFor(x => x.Value).NotEmpty();
        }
    }
}