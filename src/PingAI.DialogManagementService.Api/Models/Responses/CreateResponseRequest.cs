namespace PingAI.DialogManagementService.Api.Models.Responses
{
    public class CreateResponseRequest
    {
        public string ProjectId { get; set; }
        public string Type { get; set; }
        public string? RteText { get; set; }
        public int Order { get; set; }
    }
}