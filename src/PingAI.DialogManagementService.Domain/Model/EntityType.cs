using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class EntityType : DomainEntity, IHaveTimestamps
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
        
        public EntityType(string name, Guid projectId, string description, string[]? tags)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"{nameof(name)} cannot be empty");
            if (name.Trim().Length > MaxNameLength)
                throw new ArgumentException($"Max length of {nameof(name)} is {MaxNameLength}");
            
            if (tags != null && tags.Any(t => t == null 
                                              || t.Trim().Length == 0 || t.Trim().Length > MaxTagLength))
                throw new ArgumentException(
                    $"Some {nameof(Tags)} are not valid: empty tags and tags with " +
                    $"length > {MaxTagLength} are not allowed");

            
            Name = name.Trim();
            ProjectId = projectId;
            Description = description;
            Tags = tags?.Select(t => t.Trim()).ToArray();
            _values = new List<EntityValue>();
        }
        
        public EntityType(string name, Guid projectId, string description, string[]? tags, IEnumerable<EntityValue> values)
        {
            Name = name;
            ProjectId = projectId;
            Description = description;
            Tags = tags;
            _values = (values ?? throw new ArgumentNullException(nameof(values))).ToList();
        }

        
        public void UpdateValues(IEnumerable<EntityValue> values)
        {
            _values.Clear();
            var valuesToAdd = values.ToArray();
            foreach (var value in valuesToAdd)
            {
                value.UpdateEntityTypeId(Id);
            }
            _values.AddRange(valuesToAdd);
        }

        public override string ToString() => Name;
    }
}