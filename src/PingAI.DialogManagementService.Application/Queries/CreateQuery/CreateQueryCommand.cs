using System;
using MediatR;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Queries.CreateQuery
{
    public class CreateQueryCommand : IRequest<Query>
    {
        public string Name { get; set; }
        public Guid ProjectId { get; set; }
        public Expression[] Expressions { get; set; }
        public string Description { get; set; }
        public string[]? Tags { get; set; }
        public int DisplayOrder { get; set; }
        public Guid? IntentId { get; set; }
        public Intent? Intent { get; set; }
        public Guid? ResponseId { get; set; }
        public Response? Response { get; set; }
    }
}