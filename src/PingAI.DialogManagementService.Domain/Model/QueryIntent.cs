using System;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class QueryIntent : IHaveTimestamps
    {
        public Guid Id { get; private set; }
        // public readonly Guid _queryId;
        // public Guid QueryId => _queryId;
        // public Query? Query { get; private set; }
        // private readonly Guid _intentId;
        // public Guid IntentId => _intentId;
        // public Intent? Intent { get; private set; }

        // public override string ToString() => $"Query:{QueryId} Intent:{IntentId}";
    }
}