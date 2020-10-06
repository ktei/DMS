using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Intents
{
    public class PhrasePartDto
    {
        public string Id { get; set; }
        public string IntentId { get; set; }
        public string PhraseId { get; set; }
        public int? Position { get; set; }
        public string? Text { get; set; }
        public string? Value { get; set; }
        public string Type { get; set; }
        public string? EntityName { get; set; }
        public string? EntityTypeId { get; set; }
        public int DisplayOrder { get; set; }

        public PhrasePartDto(string id, string intentId, string phraseId, int? position, string? text, string? value,
            string type, string? entityName, string? entityTypeId, int displayOrder)
        {
            Id = id;
            IntentId = intentId;
            PhraseId = phraseId;
            Position = position;
            Text = text;
            Value = value;
            Type = type;
            EntityName = entityName;
            EntityTypeId = entityTypeId;
            DisplayOrder = displayOrder;
        }

        public PhrasePartDto(PhrasePart phrasePart) :
            this(phrasePart.Id.ToString(),
                phrasePart.IntentId.ToString(),
                phrasePart.PhraseId.ToString(),
                phrasePart.Position,
                phrasePart.Text,
                phrasePart.Value,
                phrasePart.Type.ToString(),
                phrasePart.EntityName?.Name,
                phrasePart.EntityTypeId?.ToString(),
                phrasePart.DisplayOrder)
        {
            
        }

        public PhrasePartDto()
        {
            
        }
    }
}