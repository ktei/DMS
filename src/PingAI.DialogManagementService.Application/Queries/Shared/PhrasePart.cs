using System;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Queries.Shared
{
    public class PhrasePart
    {
        public Guid PhraseId { get; }
        public PhrasePartType Type { get; }
        public int? Position { get; }
        public string Text { get; }
        public string? Value { get; }
        public string? EntityName { get; }
        public Guid? EntityTypeId { get; }

        public static PhrasePart CreateText(Guid phraseId, int position, string text)
        {
            return new PhrasePart(phraseId, PhrasePartType.TEXT,
                position, text, null, null);
        }

        public static PhrasePart CreateEntity(Guid phraseId, int position,
            string text, string entityName)
        {
            return new PhrasePart(phraseId, PhrasePartType.ENTITY,
                position, text, null, entityName);
        }

        public PhrasePart(Guid phraseId, PhrasePartType type, int? position, string? text, string? value,
            string? entityName)
        {
            PhraseId = phraseId;
            Type = type;
            Position = position;
            Text = text;
            Value = value;
            EntityName = entityName;
        }
    }
}