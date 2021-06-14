using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using PingAI.DialogManagementService.Domain.Utils;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class Query : DomainEntity
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public Guid ProjectId { get; private set; }
        public Project? Project { get; private set; }
        public Expression[] Expressions { get; private set; }
        public string Description { get; private set; }
        public string[]? Tags { get; private set; }
        public int DisplayOrder { get; private set; }
        private readonly List<Intent> _intents;
        public IReadOnlyList<Intent> Intents => _intents.ToImmutableList();
        private readonly List<Response> _responses;
        public IReadOnlyList<Response> Responses => _responses.ToImmutableList();
        
        public const int MaxNameLength = 100;
        public const int MaxTagLength = 50;
        
        // Needed by EF
        private Query(){}

        public Query(string name, IEnumerable<Expression> expressions,
            string description, IEnumerable<string>? tags, int displayOrder)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"{nameof(name)} cannot be empty.");
            if (name.Trim().Length > MaxNameLength)
                throw new ArgumentException($"Max length of {nameof(name)} is {MaxNameLength}.");

            if (tags != null)
            {
                var tagsToAdd = tags.ToArray();
                if (tagsToAdd.Any(t => t == null
                                                  || t.Trim().Length == 0 || t.Trim().Length > MaxTagLength))
                    throw new ArgumentException(
                        $"Some {nameof(Tags)} are not valid: empty tags and tags with " +
                        $"length > {MaxTagLength} are not allowed.");

                Tags = tagsToAdd.Select(t => t.Trim()).ToArray();
            }
            else
            {
                Tags = new string[0];
            }

            Name = name.Trim();
            Expressions = expressions.ToArray();
            Description = description;
            DisplayOrder = displayOrder;
            _intents = new List<Intent>();
            _responses = new List<Response>();
        }

        public Query(Guid projectId, string name, IEnumerable<Expression> expressions,
            string description, IEnumerable<string>? tags, int displayOrder)
            : this(name, expressions, description, tags, displayOrder)
        {
            if (projectId.IsEmpty())
                throw new ArithmeticException($"{nameof(projectId)} cannot be empty.");
            ProjectId = projectId;
        }

        public void AddResponse(Response response)
        {
            _ = response ?? throw new ArgumentNullException(nameof(response));
            if (_responses == null)
                throw new InvalidOperationException($"Load {nameof(Responses)} first.");
            _responses.Add(response);
        }

        public void ClearResponses()
        {
            if (_responses == null)
                throw new InvalidOperationException($"Load {nameof(Responses)} first.");
            
            _responses.Clear();
        }

        public void AddIntent(Intent intent)
        {
            _ = intent ?? throw new ArgumentNullException(nameof(intent));
            if (_intents == null)
                throw new InvalidOperationException($"Load {nameof(Intents)} first.");
            _intents.Add(intent);
        }
        
        public void ClearIntents()
        {
            if (_intents == null)
                throw new InvalidOperationException($"Load {nameof(Intents)} first");

            _intents.Clear();
        }

        public void UpdateDisplayOrder(int displayOrder)
        {
            if (displayOrder < 0)
                throw new ArgumentException($"{nameof(displayOrder)} should >= 0");
            DisplayOrder = displayOrder;
        }
        
        public override string ToString() => Name;
    }
}
