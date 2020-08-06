using System;
using System.Collections.Generic;
using MediatR;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Intents.ListIntents
{
    public class ListIntentsQuery : IRequest<List<Intent>>
    {
        public Guid? ProjectId { get; set; }

        public ListIntentsQuery(Guid? projectId)
        {
            ProjectId = projectId;
        }

        public ListIntentsQuery()
        {
            
        }
    }
}