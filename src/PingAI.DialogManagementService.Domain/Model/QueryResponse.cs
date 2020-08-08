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

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public QueryResponse(Guid queryId, Guid responseId)
        {
            QueryId = queryId;
            ResponseId = responseId;
        }

        public override string ToString() => $"Query:{QueryId}-Response:{ResponseId}";
    }
}