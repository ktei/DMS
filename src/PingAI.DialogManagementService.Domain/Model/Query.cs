using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class Query : IHaveTimestamps
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public Guid ProjectId { get; private set; }
        public Project? Project { get; private set; }
        public Expression[] Expressions { get; private set; }
        public string Description { get; private set; }
        public string[]? Tags { get; private set; }
        public int DisplayOrder { get; private set; }

        private readonly List<QueryIntent> _queryIntents = new List<QueryIntent>();
        public IReadOnlyList<QueryIntent> QueryIntents => _queryIntents.ToImmutableList();

        private readonly List<QueryResponse> _queryResponses = new List<QueryResponse>();
        public IReadOnlyList<QueryResponse> QueryResponses => _queryResponses.ToImmutableList();
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Query(Guid id, string name, Guid projectId, Expression[] expressions,
            string description, string[]? tags, int displayOrder)
        {
            Id = id;
            Name = name;
            ProjectId = projectId;
            Expressions = expressions;
            Description = description;
            Tags = tags;
            DisplayOrder = displayOrder;
        }

        public void AddResponse(Response response)
        {
            _ = response ?? throw new ArgumentNullException(nameof(response));
            _queryResponses.Add(new QueryResponse(Guid.NewGuid(), Id, response.Id));
        }

        public void AddIntent(Intent intent)
        {
            _ = intent ?? throw new ArgumentNullException(nameof(intent));
            _queryIntents.Add(new QueryIntent(Guid.NewGuid(), Id, intent.Id));
        }

        public override string ToString() => Name;
    }
}