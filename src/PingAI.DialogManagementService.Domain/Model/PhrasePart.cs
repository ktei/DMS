using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class PhrasePart : IHaveTimestamps
    {
        public Guid Id { get; private set; }
        public Guid IntentId { get; private set; }
        public Guid PhraseId { get; private set; }
        public int? Position { get; private set; }
        public Guid? EntityNameId { get; private set; }
        public Guid? EntityTypeId { get; private set; }
        public string? Text { get; private set; }
        public string? Value { get; private set; }
        public PhrasePartType Type { get; private set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public enum PhrasePartType
    {
        CONSTANT_ENTITY,
        ENTITY,
        TEXT
    }
}