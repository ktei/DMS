namespace PingAI.DialogManagementService.Api.Models.Intents
{
    public class CreateIntentRequest
    {
        public string ProjectId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public CreatePhrasePartDto[][]? Phrases { get; set; }
    }
}