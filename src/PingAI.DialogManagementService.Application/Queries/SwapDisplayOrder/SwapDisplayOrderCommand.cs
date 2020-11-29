using System;
using MediatR;

namespace PingAI.DialogManagementService.Application.Queries.SwapDisplayOrder
{
    public class SwapDisplayOrderCommand : IRequest
    {
        public Guid QueryId { get; set; }
        public Guid TargetQueryId { get; set; }

        public SwapDisplayOrderCommand(Guid queryId, Guid targetQueryId)
        {
            QueryId = queryId;
            TargetQueryId = targetQueryId;
        }

        public SwapDisplayOrderCommand()
        {
            
        }
    }
}