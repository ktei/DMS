namespace PingAI.DialogManagementService.Api.Models.Responses
{
    public class ResolutionDto
    {
        public ResolutionPartDto[]? Parts { get; set; }
        public FormResolutionDto? Form { get; set; }
        public WebhookResolutionDto? Webhook { get; set; }
        public string Type { get; set; }

        public ResolutionDto(ResolutionPartDto[]? parts, FormResolutionDto? form, 
            WebhookResolutionDto? webhook,
            string type)
        {
            Parts = parts;
            Form = form;
            Webhook = webhook;
            Type = type;
        }

        public ResolutionDto()
        {
            
        }
    }
}