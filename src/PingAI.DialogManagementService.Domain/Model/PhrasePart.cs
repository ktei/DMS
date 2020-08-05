using System;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class PhrasePart : IHaveTimestamps
    {
        public Guid Id { get; private set; }
        public Guid IntentId { get; private set; }
        public Intent? Intent { get; private set; }
        public Guid PhraseId { get; private set; }
        public int? Position { get; private set; }
        public string? Text { get; private set; }
        public string? Value { get; private set; }
        public PhrasePartType Type { get; private set; }
        public Guid? EntityNameId { get; private set; }
        public EntityName? EntityName { get; private set; }
        public Guid? EntityTypeId { get; private set; }
        public EntityType? EntityType { get; private set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public PhrasePart(Guid id, Guid intentId, Guid phraseId, int? position, string? text, string? value,
            PhrasePartType type, Guid? entityNameId, Guid? entityTypeId)
        {
            Id = id;
            IntentId = intentId;
            PhraseId = phraseId;
            Position = position;
            Text = text;
            Value = value;
            Type = type;
            EntityNameId = entityNameId;
            EntityTypeId = entityTypeId;
        }

        public void UpdateIntentId(Guid intentId) => IntentId = intentId;

        public void UpdatePosition(int position)
        {
            if (Type != PhrasePartType.CONSTANT_ENTITY)
            {
                Position = position;
            }
        }
        
        public void UpdateEntityName(EntityName entityName)
        {
            EntityName = entityName;
            EntityNameId = entityName.Id;
        }
    }

    public enum PhrasePartType
    {
        CONSTANT_ENTITY,
        ENTITY,
        TEXT
    }
}