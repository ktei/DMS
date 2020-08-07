using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class EntityType : IHaveTimestamps
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public Guid ProjectId { get; private set; }
        public Project? Project { get; private set; }
        public string Description { get; private set; }
        public string[]? Tags { get; private set; }

        private readonly List<EntityValue> _values = new List<EntityValue>();
        public IReadOnlyList<EntityValue> Values => _values.ToImmutableList();
        
        public DateTime CreatedAt { get; set; } public DateTime UpdatedAt { get; set; }

        public EntityType(Guid id, string name, Guid projectId, string description, string[]? tags)
        {
            Id = id;
            Name = name;
            ProjectId = projectId;
            Description = description;
            Tags = tags;
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