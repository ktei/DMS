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
        
        public const int MaxNameLength = 100;

        public Intent(string name, Guid projectId,
            IntentType type)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"{nameof(name)} cannot be empty");
            if (name.Trim().Length > MaxNameLength)
                throw new ArgumentException($"Max length of {nameof(name)} is {MaxNameLength}");
            
            Name = name.Trim();
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
            // Updated event only makes sense when there is no Deleted event
            if (DomainEvents.Any(e => e is IntentDeletedEvent))
                return;

            // Deduplicate
            var oldUpdateEvent = DomainEvents.FirstOrDefault(e => e is IntentUpdatedEvent);
            RemoveDomainEvent(oldUpdateEvent);
            AddDomainEvent(new IntentUpdatedEvent(this));
        }

        public IEnumerable<string> GetPhrases()
        {
            if (_phraseParts == null)
                throw new InvalidOperationException($"Load {nameof(PhraseParts)} first");
            
            var groupedParts = PhraseParts.GroupBy(p => p.PhraseId);
            var enumerable = groupedParts as IGrouping<Guid, PhrasePart>[] ?? groupedParts.ToArray();
            foreach (var group in enumerable)
            {
                yield return string.Join("", enumerable.First().OrderBy(p => p.Position)
                    .Select(p => p.Type switch
                    {
                        PhrasePartType.TEXT => p.Text!,
                        PhrasePartType.ENTITY => p.EntityName!.Name,
                        PhrasePartType.CONSTANT_ENTITY => p.Value!,
                        _ => string.Empty
                    }));
            }
        }

        public void Delete()
        {
            // If this intent is going to be deleted,
            // there's no point to have other events
            ClearDomainEvents();
            
            // TODO: soft deletion should update DeletedAt
            if (!DomainEvents.Any(e => e is IntentDeletedEvent))
            {
                AddDomainEvent(new IntentDeletedEvent(this));
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