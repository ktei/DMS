using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class ResolutionPart : ValueObject
    {
        [JsonPropertyName("path")]
        public string? Path { get; set; }
        
        [JsonPropertyName("text")]
        public string? Text { get; set; }
        
        [JsonPropertyName("entityNameId")]
        public Guid? EntityNameId { get; set; }
        
        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ResolutionPartType Type { get; set; }

        [JsonConstructor]
        public ResolutionPart(string? text, Guid? entityNameId, ResolutionPartType type, string? path)
        {
            Text = text;
            EntityNameId = entityNameId;
            Type = type;
            Path = path;
        }

        public static ResolutionPart TextPart(string text) => 
            new ResolutionPart(text ?? throw new ArgumentNullException(nameof(text)), null, ResolutionPartType.RTE, null);
        public static ResolutionPart EntityNamePart(string text, Guid entityNameId, string? path) =>
            new ResolutionPart(text ?? throw new ArgumentNullException(nameof(text)), entityNameId, ResolutionPartType.OTHER_ENTITY_NAME, path);

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Text;
            yield return EntityNameId;
            yield return Type;
        }

        public override string ToString() => Text ?? EntityNameId?.ToString() ?? base.ToString();
    }
}
