using System;
using PingAI.DialogManagementService.Domain.Utils;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class EntityValue : DomainEntity
    {
        public Guid Id { get; private set; }
        public string Value { get; private set; }
        public Guid EntityTypeId { get; private set; }
        public EntityType? EntityType { get; private set; }
        public string[]? Synonyms { get; private set; }

        public EntityValue(Guid entityTypeId, string value, string[]? synonyms)
        : this(value, synonyms)
        {
            if (entityTypeId.IsEmpty())
                throw new ArgumentException($"{nameof(entityTypeId)} cannot be empty.");
            EntityTypeId = entityTypeId;
        }
        
        public EntityValue(string value, string[]? synonyms)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{nameof(value)} cannot be empty.");
            Value = value;
            Synonyms = synonyms;
        }
        
        public override string ToString() => Value;
    }
}
