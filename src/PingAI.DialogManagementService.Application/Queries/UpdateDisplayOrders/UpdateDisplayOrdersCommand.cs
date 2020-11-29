using System;
using System.Collections.Generic;
using MediatR;

namespace PingAI.DialogManagementService.Application.Queries.UpdateDisplayOrders
{
    public class UpdateDisplayOrdersCommand : IRequest
    {
        public Guid ProjectId { get; set; }
        public Dictionary<Guid, int> DisplayOrders { get; set; }

        public UpdateDisplayOrdersCommand(Guid projectId, Dictionary<Guid, int> displayOrders)
        {
            ProjectId = projectId;
            DisplayOrders = displayOrders;
        }

        public UpdateDisplayOrdersCommand()
        {
            
        }
    }
}