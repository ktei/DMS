using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

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

        private readonly List<QueryIntent> _queryIntents;
        public IReadOnlyList<QueryIntent> QueryIntents => _queryIntents.ToImmutableList();

        private readonly List<QueryResponse> _queryResponses;
        public IReadOnlyList<QueryResponse> QueryResponses => _queryResponses.ToImmutableList();

        public IReadOnlyList<Intent> Intents => GetIntents();
        public IReadOnlyList<Response> Responses => GetResponses();
        
        public Query(string name, Guid projectId, Expression[] expressions,
            string description, string[]? tags, int displayOrder)
        {
            Name = name;
            ProjectId = projectId;
            Expressions = expressions;
            Description = description;
            Tags = tags;
            DisplayOrder = displayOrder;
            _queryIntents = new List<QueryIntent>();
            _queryResponses = new List<QueryResponse>();
        }

        public void AddResponse(Response response)
        {
            _ = response ?? throw new ArgumentNullException(nameof(response));
            _queryResponses.Add(new QueryResponse(Id, response));
        }

        public void ClearResponses()
        {
            if (_queryResponses == null)
                throw new InvalidOperationException($"Load {nameof(QueryResponses)} first");
            
            _queryResponses.Clear();
        }

        public void AddIntent(Intent intent)
        {
            _ = intent ?? throw new ArgumentNullException(nameof(intent));
            _queryIntents.Add(new QueryIntent(Id, intent));
        }
        
        public void ClearIntents()
        {
            if (_queryIntents == null)
                throw new InvalidOperationException($"Load {nameof(QueryIntents)} first");

            foreach (var queryIntent in _queryIntents)
            {
                queryIntent.Intent!.Delete();
            }
            _queryIntents.Clear();
        }

        public void Delete()
        {
            if (_queryIntents == null)
                throw new InvalidOperationException($"Load {nameof(QueryIntents)} first");

            foreach (var intent in _queryIntents.Select(x => x.Intent))
            {
                if (intent == null)
                    throw new InvalidOperationException($"Load {nameof(QueryIntents)}.Intent first");
                intent.Delete();
            }
        }

        private IReadOnlyList<Intent> GetIntents()
        {
            if (_queryIntents == null)
                throw new InvalidOperationException($"Load {nameof(QueryIntents)} first");
            
            if (_queryIntents.Any(x => x.Intent == null))
                throw new InvalidOperationException($"Load {nameof(QueryIntents)}.Intent first");

            return _queryIntents.Select(x => x.Intent!).ToImmutableList();
        }

        private IReadOnlyList<Response> GetResponses()
        {
            if (_queryResponses == null)
                throw new InvalidOperationException($"Load {nameof(QueryResponses)} first");
            
            if (_queryResponses.Any(x => x.Response == null))
                throw new InvalidOperationException($"Load {nameof(QueryResponses)}.Response first");

            return _queryResponses.Select(x => x.Response!).ToImmutableList();
        }

        public override string ToString() => Name;
    }
}