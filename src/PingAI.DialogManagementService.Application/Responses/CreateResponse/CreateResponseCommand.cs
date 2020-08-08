using System;
using MediatR;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Responses.CreateResponse
{
    public class CreateResponseCommand : IRequest<Response>
    {
        public Guid ProjectId { get; set; }
        public ResponseType Type { get; set; }
        public string? RteText { get; set; }
        public int Order { get; set; }

        public CreateResponseCommand(Guid projectId, ResponseType type, string? rteText, int order)
        {
            ProjectId = projectId;
            Type = type;
            RteText = rteText;
            Order = order;
        }

        public CreateResponseCommand()
        {
            
        }
    }
}