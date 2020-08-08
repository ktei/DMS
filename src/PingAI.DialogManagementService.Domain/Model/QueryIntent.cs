using System;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class QueryIntent : IHaveTimestamps
    {
        public Guid Id { get; private set; }
        public Guid QueryId { get; private set; }
        public Query? Query { get; private set; }
        public Guid IntentId { get; private set; }
        public Intent? Intent { get; private set; }

        public QueryIntent(Guid queryId, Guid intentId)
        {
            QueryId = queryId;
            IntentId = intentId;
        }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        public override string ToString() => $"Query:{QueryId}-Intent:{IntentId}";
    }
}