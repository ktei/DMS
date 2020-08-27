using System;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class EntityName : DomainEntity, IHaveTimestamps
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public Guid ProjectId { get; private set; }
        public Project? Project { get; private set; }
        public bool CanBeReferenced { get; private set; }

        public const int MaxNameLength = 30; // Limited by Dialogflow
        
        public EntityName(string name, Guid projectId, bool canBeReferenced)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"{nameof(name)} cannot be empty");
            if (name.Trim().Length > MaxNameLength)
                throw new ArgumentException($"Max length of {nameof(name)} is {MaxNameLength}");
            
            Name = name.Trim();
            ProjectId = projectId;
            CanBeReferenced = canBeReferenced;
        }

        public override string ToString() => Name;
    }
}