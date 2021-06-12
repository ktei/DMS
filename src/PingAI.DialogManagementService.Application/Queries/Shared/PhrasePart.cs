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