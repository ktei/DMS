namespace PingAI.DialogManagementService.Api.Models.Intents
{
    public class CreatePhrasePartDto
    {
        public int? Position { get; set; }
        public string Text { get; set; }
        public string? Value { get; set; }
        public string Type { get; set; }
        public string? EntityName { get; set; }
        public string? EntityTypeId { get; set; }
    }
}