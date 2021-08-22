namespace PingAI.DialogManagementService.Api.Models.Intents
{
    public class UpdateIntentRequest
    {
        public string Name { get; set; }
        public CreatePhrasePartDto[][]? Phrases { get; set; }
    }
}