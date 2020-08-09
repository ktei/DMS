using System;
using MediatR;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Queries.UpdateQuery
{
    public class UpdateQueryCommand : IRequest<Query>
    {
        public Guid QueryId { get; set; }
        public string Name { get; set; }
        public Expression[] Expressions { get; set; }
        public string Description { get; set; }
        public string[]? Tags { get; set; }
        public int DisplayOrder { get; set; }
        public Guid? IntentId { get; set; }
        public Intent? Intent { get; set; }
        public Guid? ResponseId { get; set; }
        public Response? Response { get; set; }
        public string? RteText { get; set; } 
        
        public UpdateQueryCommand(Guid queryId, string name, Expression[] expressions, string description,
            string[]? tags, int displayOrder, Guid? intentId, Intent? intent, Guid? responseId, Response? response,
            string? rteText)
        {
            QueryId = queryId;
            Name = name;
            Expressions = expressions;
            Description = description;
            Tags = tags;
            DisplayOrder = displayOrder;
            IntentId = intentId;
            Intent = intent;
            ResponseId = responseId;
            Response = response;
            RteText = rteText;
        }

        public UpdateQueryCommand()
        {
            
        }
    }
}