using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Responses
{
    public class CreateResponseResponse : ResponseDto
    {
        public CreateResponseResponse(string responseId, string projectId, string type, ResolutionPartDto[] resolution)
            : base(responseId, projectId, type, resolution)
        {
        }

        public CreateResponseResponse(Response response) : base(response)
        {
            
        }

        public CreateResponseResponse()
        {
            
        }
    }
}