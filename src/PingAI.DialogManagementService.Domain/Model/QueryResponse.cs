using System;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class QueryResponse : IHaveTimestamps
    {
        public Guid Id { get; private set; }
        public Guid QueryId { get; private set; }
        public Query? Query { get; private set; }
        public Guid ResponseId { get; private set; }
        public Response? Response { get; private set; }

        public QueryResponse(Guid queryId, Guid responseId)
        {
            QueryId = queryId;
            ResponseId = responseId;
        }

        public QueryResponse(Guid queryId, Response response)
        {
            QueryId = queryId;
            ResponseId = response.Id;
            Response = response;
        }

        public override string ToString() => $"Query:{QueryId}-Response:{ResponseId}";
    }
}