using System;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class EntityName : IHaveTimestamps
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public Guid ProjectId { get; private set; }
        public Project? Project { get; private set; }
        public bool CanBeReferenced { get; private set; }
        
        public EntityName(string name, Guid projectId, bool canBeReferenced)
        {
            Name = name;
            ProjectId = projectId;
            CanBeReferenced = canBeReferenced;
        }

        public override string ToString() => Name;
    }
}