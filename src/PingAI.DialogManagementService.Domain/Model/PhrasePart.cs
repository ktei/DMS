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

        public const int MaxValueLength = 255;
        public const int MaxTextLength = 255;
        
        public PhrasePart(Guid intentId, Guid phraseId, int? position, string? text, string? value,
            PhrasePartType type, Guid? entityNameId, Guid? entityTypeId)
        {
            if (Text?.Length > MaxTextLength)
                throw new ArgumentException($"Max length of {nameof(Text)} is {MaxTextLength}");
            
            if (Value?.Length > MaxValueLength)
                throw new ArgumentException($"Max length of {nameof(Value)} is {MaxValueLength}");
            
            IntentId = intentId;
            PhraseId = phraseId;
            Position = position;
            Text = text;
            Value = value;
            Type = type;
            EntityNameId = entityNameId;
            EntityTypeId = entityTypeId;
        }
        
        public PhrasePart(Guid intentId, Guid phraseId, int? position, string? text, string? value,
            PhrasePartType type, EntityName? entityName, Guid? entityTypeId) : this(intentId,
            phraseId, position, text, value, type, default(Guid?), entityTypeId)
        {
            EntityName = entityName;
        }
        
        public PhrasePart(Guid intentId, Guid phraseId, int? position, string? text, string? value,
            PhrasePartType type, EntityName? entityName, EntityType? entityType) : this(intentId,
            phraseId, position, text, value, type, default(Guid?), default(Guid?))
        {
            EntityName = entityName;
            EntityType = entityType;
        }

        public void UpdatePosition(int position)
        {
            if (Type != PhrasePartType.CONSTANT_ENTITY)
            {
                Position = position;
            }
        }
        
        public void UpdateEntityName(EntityName entityName)
        {
            _ = entityName ?? throw new ArgumentNullException(nameof(entityName));
            EntityName = entityName;
            EntityNameId = entityName.Id;
        }

        public override string ToString() => Text ?? Value ?? string.Empty;
    }

    public enum PhrasePartType
    {
        CONSTANT_ENTITY,
        ENTITY,
        TEXT
    }
}