namespace PingAI.DialogManagementService.Api.Models.Responses
{
    public class WebhookResolutionDto
    {
        public string Name { get; set; }
        public string ResponseId { get; set; }

        public WebhookResolutionDto(string name, string responseId)
        {
            Name = name;
            ResponseId = responseId;
        }

        public WebhookResolutionDto()
        {
            
        }
    }
}