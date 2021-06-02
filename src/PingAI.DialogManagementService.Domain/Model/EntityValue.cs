using System;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class EntityValue : DomainEntity, IHaveTimestamps
    {
        public Guid Id { get; private set; }
        public string Value { get; private set; }
        public Guid EntityTypeId { get; private set; }
        public EntityType? EntityType { get; private set; }
        public string[]? Synonyms { get; private set; }

        public EntityValue(string value, Guid entityTypeId, string[]? synonyms)
        {
            Value = value;
            EntityTypeId = entityTypeId;
            Synonyms = synonyms;
        }
        
        public void UpdateEntityTypeId(Guid entityTypeId) => EntityTypeId = entityTypeId;

        public override string ToString() => Value;
    }
}