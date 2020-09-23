using System.Linq;
using System.Text.Json.Serialization;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Responses
{
    public class ResponseDto
    {
        public string ResponseId { get; set; }
        public string ProjectId { get; set; }
        public string Type { get; set; }
        
        [JsonPropertyName("resolution")]
        public ResolutionPartDto[]? Parts { get; set; }
        
        [JsonPropertyName("form")]
        public FormResolutionDto? Form { get; set; }

        public ResponseDto(Response response)
        {
            ResponseId = response.Id.ToString();
            ProjectId = response.ProjectId.ToString();
            Type = response.Type.ToString();
            if (response.Resolution?.Type == ResolutionType.PARTS)
            {
                Parts = response.Resolution.Parts?.Select(p => new ResolutionPartDto(p)).ToArray();
            }
            else if (response.Resolution?.Type == ResolutionType.FORM)
            {
                Form = response.Resolution.Form == null ? default : new FormResolutionDto(response.Resolution.Form);
            }
        }

        public ResponseDto()
        {
            
        }
    }
}