using System;
using System.Text.Json.Serialization;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class Resolution
    {
        [JsonPropertyName("type")]
        public ResolutionType Type { get; set; }
       
        [JsonPropertyName("parts")]
        public ResolutionPart[]? Parts { get; set; }
       
        [JsonPropertyName("form")]
        public FormResolution? Form { get; set; }

        public Resolution(ResolutionPart[] parts)
        {
            Parts = parts ?? throw new ArgumentNullException(nameof(parts));
            Type = ResolutionType.PARTS;
        }

        public Resolution(FormResolution form)
        {
            Form = form ?? throw new ArgumentNullException(nameof(form));
            Type = ResolutionType.FORM;
        }

        public Resolution()
        {
            
        }
    }
    
    public enum ResolutionType
    {
        PARTS,
        FORM
    }
}