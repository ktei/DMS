using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class FormResolution
    {
        [JsonPropertyName("fields")]
        public Field[] Fields { get; set; }

        public FormResolution()
        {
            
        }

        public FormResolution(Field[] fields)
        {
            Fields = fields ?? new Field[0];
        }
        
        public class Field
        {
            [JsonPropertyName("displayName")]
            public string DisplayName { get; set; }
            
            [JsonPropertyName("name")]
            public string Name { get; set; }
            
            [JsonPropertyName("defaultValue")]
            public string? DefaultValue { get; set; }
        }
    }
}