using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using MediatR;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Queries.CreateQuery
{
    public class PhrasePart
    {
        public Guid PhraseId { get; }
        public PhrasePartType Type { get; }
        public int? Position { get; }
        public string Text { get; }
        public string? Value { get; }
        public string? EntityName { get; }
        public Guid? EntityTypeId { get; }

        public PhrasePart(Guid phraseId, PhrasePartType type, int? position, string? text, string? value,
            string? entityName)
        {
            PhraseId = phraseId;
            Type = type;
            Position = position;
            Text = text;
            Value = value;
            EntityName = entityName;
        }
    }

    public class Response
    {
        public string? RteText { get; }
        public FormResolution? Form { get; }
        public int Order { get; }

        public Response(string? rteText, FormResolution? form, int order)
        {
            RteText = rteText;
            Form = form;
            Order = order;
        }
    }
    
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
