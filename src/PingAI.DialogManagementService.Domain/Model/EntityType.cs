using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using PingAI.DialogManagementService.Domain.Utils;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class EntityType : DomainEntity
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public Guid ProjectId { get; private set; }
        public Project? Project { get; private set; }
        public string Description { get; private set; }
        public string[]? Tags { get; private set; }

        private readonly List<EntityValue> _values;
        public IReadOnlyList<EntityValue> Values => _values.ToImmutableList();

        public const int MaxNameLength = 30;
        public const int MaxTagLength = 50;

        public EntityType(Guid projectId, string name, string description)
            : this(name, description)
        {
            if (projectId.IsEmpty())
                throw new ArgumentException($"{nameof(projectId)} cannot be empty.");
            ProjectId = projectId;
        }

        public EntityType(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"{nameof(name)} cannot be empty");
            if (name.Trim().Length > MaxNameLength)
                throw new ArgumentException($"Max length of {nameof(name)} is {MaxNameLength}");

            Id = Guid.NewGuid();
            Name = name.Trim();
            Description = description;
            _values = new List<EntityValue>();
        }

        public void AddValue(string value, string[]? synonyms)
        {
            var entityValue = new EntityValue(value, synonyms);
            _values.Add(entityValue);
        }

        public override string ToString() => Name;
    }
}