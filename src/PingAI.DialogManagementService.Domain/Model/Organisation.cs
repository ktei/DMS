using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class Organisation : DomainEntity
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string[]? Tags { get; private set; }

        public DateTime CreatedAt { get; private set; }
        
        private readonly List<Project> _projects;
        public IReadOnlyList<Project> Projects => _projects.ToImmutableList();

        private readonly List<User> _users;
        public IReadOnlyList<User> Users => _users.ToImmutableList();
        
        public const int MaxNameLength = 250;
        
        public Organisation(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"{nameof(name)} cannot be empty");
            if (name.Trim().Length > MaxNameLength)
                throw new ArgumentException($"Max length of {nameof(name)} is {MaxNameLength}");
            
            Name = name.Trim();
            Description = description;
            _projects = new List<Project>();
            _users = new List<User>();
        }
        
        public void AddProject(Project project)
        {
            _ = project ?? throw new ArgumentNullException(nameof(project));
            if (_projects == null)
                throw new InvalidOperationException($"Load {nameof(Projects)} first");

            _projects.Add(project);
        }

        public void AddProjectVersion(ProjectVersion projectVersion)
        {
            throw new NotImplementedException();
        }

        public void AddUser(User user)
        {
            _ = user ?? throw new ArgumentNullException(nameof(user));
            if (_users == null)
                throw new InvalidOperationException($"Load {nameof(Users)} first");

            _users.Add(user);
        }

        public void RemoveUser(User user)
        {
            _ = user ?? throw new ArgumentNullException(nameof(user)); 
            if (_users == null)
                throw new InvalidOperationException($"Load {nameof(Users)} first");

            _users.Remove(user);
        }

        public override string ToString() => Name;
    }
}
