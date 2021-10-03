namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class CreateResponseDto
    {
        public string Type { get; set; }
        public string? RteText { get; set; }
        public CreateResponseFormDto? Form { get; set; }
        public CreateWebhookDto? Webhook { get; set; }
        public int Order { get; set; }
    }
}