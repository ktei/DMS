using System;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class QueryResponse : IHaveTimestamps
    {
        public Guid Id { get; private set; }
        // public readonly Guid _queryId;
        // public Guid QueryId => _queryId;
        // public Query? Query { get; private set; }
        // private readonly Guid _responseId;
        // public Guid ResponseId => _responseId;
        // public Response? Response { get; private set; }
        //
        // public QueryResponse(Guid queryId, Guid responseId)
        // {
        //     _queryId = queryId;
        //     _responseId = responseId;
        // }
        //
        // public override string ToString() => $"Query:{QueryId} Response:{ResponseId}";
    }
}