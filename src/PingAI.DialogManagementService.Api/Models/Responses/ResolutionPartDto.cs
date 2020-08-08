using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Responses
{
    public class ResolutionPartDto
    {
        public string? Text { get; set; }
        public string? EntityNameId { get; set; }
        public string Type { get; set; }

        public ResolutionPartDto(string? text, string? entityNameId, string type)
        {
            Text = text;
            EntityNameId = entityNameId;
            Type = type;
        }

        public ResolutionPartDto(ResolutionPart resolutionPart) : this(
            resolutionPart.Text, resolutionPart.EntityNameId?.ToString(), resolutionPart.Type.ToString())
        {
        }

        public ResolutionPartDto()
        {
            
        }
    }
}