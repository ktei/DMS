using System;
using System.Collections.Generic;

namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class UpdateDisplayOrdersRequest
    {
        public Guid ProjectId { get; set; }
        public Dictionary<Guid, int> DisplayOrders { get; set; } = new Dictionary<Guid, int>();

        public UpdateDisplayOrdersRequest(Guid projectId, Dictionary<Guid, int> displayOrders)
        {
            ProjectId = projectId;
            DisplayOrders = displayOrders;
        }

        public UpdateDisplayOrdersRequest()
        {
            
        }
    }
}