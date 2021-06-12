using System;
using System.Collections.Generic;
using MediatR;
using PingAI.DialogManagementService.Domain.Model;
using PhrasePart = PingAI.DialogManagementService.Application.Queries.Shared.PhrasePart;
using Response = PingAI.DialogManagementService.Application.Queries.Shared.Response;

namespace PingAI.DialogManagementService.Application.Queries.UpdateQuery
{
    public class UpdateQueryCommand : IRequest<Query>
    {
        public Guid QueryId { get; }
        public string Name { get; }
        public IReadOnlyList<PhrasePart> PhraseParts { get; }
        public IReadOnlyList<Expression> Expressions { get; }
        public IReadOnlyList<Response> Responses { get; }
        public string Description { get; }
        public string[]? Tags { get; }
        public int DisplayOrder { get; }

        public UpdateQueryCommand(Guid queryId, string name, IReadOnlyList<PhrasePart> phraseParts,
            IReadOnlyList<Expression> expressions, IReadOnlyList<Response> responses, string description,
            string[]? tags, int displayOrder)
        {
            QueryId = queryId;
            Name = name;
            PhraseParts = phraseParts;
            Expressions = expressions;
            Responses = responses;
            Description = description;
            Tags = tags;
            DisplayOrder = displayOrder;
        }
    }
}
