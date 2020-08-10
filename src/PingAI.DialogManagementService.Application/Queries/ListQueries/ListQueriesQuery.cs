using System;
using System.Collections.Generic;
using MediatR;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Queries.ListQueries
{
    public class ListQueriesQuery : IRequest<List<Query>>
    {
        public Guid ProjectId { get; set; }

        public ListQueriesQuery(Guid projectId)
        {
            ProjectId = projectId;
        }

        public ListQueriesQuery()
        {
            
        }
    }
}