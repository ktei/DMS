using System;
using MediatR;

namespace PingAI.DialogManagementService.Application.Queries.DeleteQuery
{
    public class DeleteQueryCommand : IRequest
    {
        public Guid QueryId { get; }

        public DeleteQueryCommand(Guid queryId)
        {
            QueryId = queryId;
        }
    }
}
