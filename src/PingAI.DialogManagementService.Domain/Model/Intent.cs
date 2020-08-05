using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class Intent : IHaveTimestamps
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public Guid ProjectId { get; private set; }
        public Project? Project { get; private set; }
        private string _iconName = string.Empty;
        private string _color = string.Empty;
        public IntentType Type { get; private set; }
        
        private readonly List<PhrasePart> _phraseParts = new List<PhrasePart>();
        public IReadOnlyList<PhrasePart> PhraseParts => _phraseParts.ToImmutableList();

        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Intent(Guid id, string name, Guid projectId,
            IntentType type)
        {
            Id = id;
            Name = name;
            ProjectId = projectId;
            Type = type;
        }
    }

    public enum IntentType
    {
        STANDARD,
        GENERIC
    }
}