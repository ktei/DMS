using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Intents
{
    public class CreatePhrasePartDto
    {
        public int? Position { get; set; }
        public string Text { get; set; }
        public string? Value { get; set; }
        public PhrasePartType Type { get; set; }
        public string? EntityName { get; set; }
        public string? EntityType { get; set; }
    }
}