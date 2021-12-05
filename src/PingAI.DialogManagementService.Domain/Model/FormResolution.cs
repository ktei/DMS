using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class FormResolution
    {
        [JsonPropertyName("fields")]
        public FormField[] Fields { get; set; }

        [JsonConstructor]
        public FormResolution(FormField[] fields)
        {
            Fields = (fields ?? throw new ArgumentNullException(nameof(fields))).ToArray();
        }
    }
    
    public class FormField
    {
        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }
            
        [JsonPropertyName("name")]
        public string Name { get; set; }
       
        [JsonPropertyName("entityNameId")]
        public Guid EntityNameId { get; set; }
            
        [JsonPropertyName("defaultValue")]
        public string? DefaultValue { get; set; }

        [JsonConstructor]
        public FormField(string displayName, string name, Guid entityNameId)
        {
            DisplayName = displayName;
            Name = name;
            EntityNameId = entityNameId;
        }
    }
}
