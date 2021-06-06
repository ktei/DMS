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

        private DateTime _createdAt;
        public DateTime CreatedAt
        {
            get => DateTime.SpecifyKind(_createdAt, DateTimeKind.Utc);
            private set => _createdAt = value;
        }
        
        private readonly List<Project> _projects;
        public IReadOnlyList<Project> Projects => _projects.ToImmutableList();

        private readonly List<ProjectVersion> _projectVersions;
        public IReadOnlyList<ProjectVersion> ProjectVersions => _projectVersions.ToImmutableList();

        private readonly List<OrganisationUser> _organisationUsers;
        public IReadOnlyList<OrganisationUser> OrganisationUsers => _organisationUsers.ToImmutableList();
        
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
            _projectVersions = new List<ProjectVersion>();
            _organisationUsers = new List<OrganisationUser>();
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
            _ = projectVersion ?? throw new ArgumentNullException(nameof(projectVersion));
            if (_projectVersions == null)
                throw new InvalidOperationException($"Load {nameof(ProjectVersions)} first");
            
            _projectVersions.Add(projectVersion);
        }

        public void AddUser(User user)
        {
            _ = user ?? throw new ArgumentNullException(nameof(user));
            if (_organisationUsers == null)
                throw new InvalidOperationException($"Load {nameof(OrganisationUsers)} first");

            _organisationUsers.Add(new OrganisationUser(Id, user));
        }

        public void RemoveUser(User user)
        {
            _ = user ?? throw new ArgumentNullException(nameof(user)); 
            if (_organisationUsers == null)
                throw new InvalidOperationException($"Load {nameof(OrganisationUsers)} first");

            _organisationUsers.RemoveAll(x => x.UserId == user.Id);
        }

        public override string ToString() => Name;
    }
}