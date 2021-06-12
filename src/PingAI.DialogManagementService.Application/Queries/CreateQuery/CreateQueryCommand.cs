using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using MediatR;
using PingAI.DialogManagementService.Domain.Model;
using PhrasePart = PingAI.DialogManagementService.Application.Queries.Shared.PhrasePart;
using Response = PingAI.DialogManagementService.Application.Queries.Shared.Response;

namespace PingAI.DialogManagementService.Application.Queries.CreateQuery
{
    public class CreateQueryCommand : IRequest<Query>
    {
        public string Name { get; }
        public Guid ProjectId { get; }
        public IReadOnlyList<PhrasePart> PhraseParts { get; }
        public IReadOnlyList<Expression> Expressions { get; }
        public IReadOnlyList<Response> Responses { get; }
        public string Description { get; }
        public IReadOnlyList<string>? Tags { get; }

        public CreateQueryCommand(string name, Guid projectId, IEnumerable<PhrasePart> phraseParts,
            IEnumerable<Expression> expressions, IEnumerable<Response> responses, string description, IEnumerable<string>? tags)
        {
            Name = name;
            ProjectId = projectId;
            PhraseParts = phraseParts.ToImmutableList();
            Expressions = expressions.ToImmutableList();
            Responses = responses.ToImmutableList();
            Description = description;
            Tags = tags?.ToImmutableList();
        }
    }
}
