using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Responses
{
    public class CreateResponseResponse : ResponseDto
    {
        public CreateResponseResponse(Response response) : base(response)
        {
            
        }

        public CreateResponseResponse()
        {
            
        }
    }
}