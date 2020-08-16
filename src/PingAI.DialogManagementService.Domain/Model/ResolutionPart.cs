using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class ResolutionPart : ValueObject
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }
        
        [JsonPropertyName("entityNameId")]
        public Guid? EntityNameId { get; set; }
        
        [JsonPropertyName("type")]
        public ResolutionPartType Type { get; set; }

        public ResolutionPart(string? text, Guid? entityNameId, ResolutionPartType type)
        {
            Text = text;
            EntityNameId = entityNameId;
            Type = type;
        }

        public ResolutionPart()
        {
            
        }
        
        public static ResolutionPart TextPart(string text) => 
            new ResolutionPart(text ?? throw new ArgumentNullException(nameof(text)), null, ResolutionPartType.RTE);
        public static ResolutionPart EntityNamePart(Guid entityNameId) =>
            new ResolutionPart(null, entityNameId, ResolutionPartType.OTHER_ENTITY_NAME);

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Text;
            yield return EntityNameId;
            yield return Type;
        }

        public override string ToString() => Text ?? EntityNameId?.ToString() ?? string.Empty;
    }
    
    public enum ResolutionPartType
    {
        RTE,
        OTHER_ENTITY_NAME 
    }
}