using System;
using System.Collections.Generic;
using MediatR;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Queries.ListQueries
{
    public class ListQueriesQuery : IRequest<IReadOnlyList<Query>>
    {
        public Guid ProjectId { get; }
        public string? QueryType { get; }
        public bool Runtime { get; }

        public ListQueriesQuery(Guid projectId, string? queryType, bool runtime)
        {
            ProjectId = projectId;
            QueryType = queryType;
            Runtime = runtime;
        }
    }
}