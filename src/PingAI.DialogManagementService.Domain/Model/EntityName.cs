using System;
using PingAI.DialogManagementService.Domain.Utils;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class EntityName : DomainEntity
    {
        public Guid Id { get; private set; }
        private readonly string _name;
        public string Name => _name;
        public Guid ProjectId { get; private set; }
        public Project? Project { get; private set; }
        private readonly bool _canBeReferenced;
        public bool CanBeReferenced => _canBeReferenced;

        public const int MaxNameLength = 30; // Limited by Dialogflow

        public EntityName(Guid projectId, string name, bool canBeReferenced)
            : this(name, canBeReferenced)
        {
            if (projectId.IsEmpty())
                throw new ArgumentException($"{nameof(projectId)} cannot be empty.");

            ProjectId = projectId;
        }

        public EntityName(string name, bool canBeReferenced)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"{nameof(name)} cannot be empty.");
            if (name.Trim().Length > MaxNameLength)
                throw new ArgumentException($"Max length of {nameof(name)} is {MaxNameLength}.");

            Id = Guid.NewGuid();
            _name = name.Trim();
            _canBeReferenced = canBeReferenced;
        }

        public override string ToString() => Name;
    }
}
