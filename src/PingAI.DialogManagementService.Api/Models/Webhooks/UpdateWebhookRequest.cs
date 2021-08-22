using System.Collections.Generic;
using FluentValidation;

namespace PingAI.DialogManagementService.Api.Models.Webhooks
{
    public class UpdateWebhookRequest
    {
        public string Method { get; set; }
        public string Url { get; set; }
        public List<KeyValuePair<string, string>> Headers { get; set; }
        
        public class UpdateWebhookRequestValidator : AbstractValidator<UpdateWebhookRequest>
        {
            public UpdateWebhookRequestValidator()
            {
                RuleFor(x => x.Method)
                    .NotEmpty();
                RuleFor(x => x.Url)
                    .NotEmpty();
                RuleFor(x => x.Headers)
                    .ForEach(x => x.SetValidator(new WebhookHeaderValidator()))
                    .When(x => x != null);
            }
        }
    }
}