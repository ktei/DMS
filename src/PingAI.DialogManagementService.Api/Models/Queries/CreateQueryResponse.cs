using PingAI.DialogManagementService.Api.Models.Intents;
using PingAI.DialogManagementService.Api.Models.Responses;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class CreateQueryResponse : QueryDto
    {
        public CreateQueryResponse(string queryId, string name, string projectId, ExpressionDto[] expressions,
            string description, string[]? tags, int displayOrder, IntentDto[] intents, ResponseDto[] responses) : base(
            queryId, name, projectId, expressions, description, tags, displayOrder, intents, responses)
        {
        }

        public CreateQueryResponse(Query query) : base(query)
        {
        }

        public CreateQueryResponse()
        {
            
        }
    }
}