using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using PingAI.DialogManagementService.Domain.Events;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class Intent : DomainEntity, IHaveTimestamps
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

        public Intent(string name, Guid projectId, IntentType type, IEnumerable<PhrasePart> phraseParts)
            : this(name, projectId, type)
        {
            UpdatePhrases(phraseParts);
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

            if (Name == name) return;
            
            Name = name;
            AddIntentUpdatedEvent();
        }

        public void UpdatePhrases(IEnumerable<PhrasePart> phraseParts)
        {
            _phraseParts.Clear();
            var partsToAdd = phraseParts.ToArray();
            foreach (var groupedParts in partsToAdd.GroupBy(p => p.PhraseId))
            {
                var position = 0;
                foreach (var part in groupedParts)
                {
                    part.UpdatePosition(position++);
                    _phraseParts.Add(part);
                }
            }

            AddIntentUpdatedEvent();
        }

        private void AddIntentUpdatedEvent()
        {
            if (!DomainEvents.Any(e => e is IntentUpdatedEvent))
            {
                AddDomainEvent(new IntentUpdatedEvent(Id, ProjectId, Name, PhraseParts.ToArray()));
            } 
        }
        
        public override string ToString() => Name;
    }

    public enum IntentType
    {
        STANDARD,
        GENERIC
    }
}