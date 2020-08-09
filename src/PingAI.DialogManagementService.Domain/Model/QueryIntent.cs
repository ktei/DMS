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

        public QueryIntent(Guid queryId, Intent intent)
        {
            QueryId = queryId;
            Intent = intent ?? throw new ArgumentNullException(nameof(intent));
            IntentId = intent.Id;
        }

        public override string ToString() => $"Query:{QueryId}-Intent:{IntentId}";
    }
}