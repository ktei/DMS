using PingAI.DialogManagementService.Api.Models.Intents;

namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class CreateIntentDto
    {
        public string Name { get; set; }
        public CreatePhrasePartDto[][] PhraseParts { get; set; }
    }
}