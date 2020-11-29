using System;
using MediatR;

namespace PingAI.DialogManagementService.Application.Queries.Insert
{
    public class InsertCommand : IRequest
    {
        public Guid QueryId { get; set; }
        public int DisplayOrder { get; set; }

        public InsertCommand(Guid queryId, int displayOrder)
        {
            QueryId = queryId;
            DisplayOrder = displayOrder;
        }

        public InsertCommand()
        {
            
        }
    }
}