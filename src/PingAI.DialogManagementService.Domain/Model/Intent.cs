using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class Intent : IHaveTimestamps
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public Guid ProjectId { get; private set; }
        public Project? Project { get; private set; }
        private string _iconName = string.Empty;
        private string _color = string.Empty;
        public IntentType Type { get; private set; }

        private readonly List<PhrasePart> _phraseParts;
        public IReadOnlyList<PhrasePart> PhraseParts => _phraseParts.ToImmutableList();

        private readonly List<QueryIntent> _queryIntents;
        public IReadOnlyList<QueryIntent> QueryIntents => _queryIntents.ToImmutableList();
        
        private const int MaxNameLength = 255;

        public Intent(string name, Guid projectId,
            IntentType type)
        {
            Name = name;
            ProjectId = projectId;
            Type = type;
            _phraseParts = new List<PhrasePart>();
            _queryIntents = new List<QueryIntent>();
        }
        
        public Intent(string name,
            IntentType type) : this(name, Guid.Empty, type)
        {
        }

        public void UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(nameof(name));
            }

            if (name.Length > MaxNameLength)
            {
                throw new ArgumentException($"{nameof(name)}'s max length is 255");
            }

            Name = name;
        }

        public void UpdatePhrases(IEnumerable<PhrasePart> phraseParts)
        {
            _phraseParts.Clear();
            var partsToAdd = phraseParts.ToArray();
            for (var i = 0; i < partsToAdd.Length; i++)
            {
                var part = partsToAdd[i];
                part.UpdatePosition(i);
                part.UpdateIntentId(Id);
            }
            _phraseParts.AddRange(partsToAdd);
        }

        public override string ToString() => Name;
    }

    public enum IntentType
    {
        STANDARD,
        GENERIC
    }
}