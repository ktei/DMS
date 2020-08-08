using System.Linq;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Responses
{
    public class ResponseDto
    {
        public string ResponseId { get; set; }
        public string ProjectId { get; set; }
        public string Type { get; set; }
        public ResolutionPartDto[] Resolution { get; set; }

        public ResponseDto(string responseId, string projectId, string type, ResolutionPartDto[] resolution)
        {
            ResponseId = responseId;
            ProjectId = projectId;
            Type = type;
            Resolution = resolution;
        }

        public ResponseDto(Response response) : this(response.Id.ToString(),
            response.ProjectId.ToString(), response.Type.ToString(), 
            response.Resolution.Select(p => new ResolutionPartDto(p)).ToArray())
        {
            
        }

        public ResponseDto()
        {
            
        }
    }
}