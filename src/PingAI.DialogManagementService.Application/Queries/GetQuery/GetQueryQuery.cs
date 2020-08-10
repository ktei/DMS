using System;
using MediatR;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Queries.GetQuery
{
    public class GetQueryQuery : IRequest<Query>
    {
        public Guid QueryId { get; set; }

        public GetQueryQuery(Guid queryId)
        {
            QueryId = queryId;
        }

        public GetQueryQuery()
        {
            
        }
    }
}