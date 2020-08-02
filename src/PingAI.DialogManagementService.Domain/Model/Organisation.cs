using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class Organisation : IHaveTimestamps
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string[] Tags { get; private set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        private readonly List<Project> _projects = new List<Project>();
        public IReadOnlyList<Project> Projects => _projects.ToImmutableList();
        
        public Organisation(Guid id, string name, string description, string[]? tags)
        {
            Id = id;
            Name = name;
            Description = description;
            Tags = tags ?? new string[0];
        }

        public void AddProject(Project project) =>
            _projects.Add(project ?? throw new ArgumentNullException(nameof(project)));

        public override string ToString() => Name;
    }
}