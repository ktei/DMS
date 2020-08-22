using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class Organisation : DomainEntity, IHaveTimestamps
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string[]? Tags { get; private set; }

        private readonly List<Project> _projects = new List<Project>();
        public IReadOnlyList<Project> Projects => _projects.ToImmutableList();

        private readonly List<OrganisationUser> _organisationUsers = new List<OrganisationUser>();
        public IReadOnlyList<OrganisationUser> OrganisationUsers => _organisationUsers.ToImmutableList();
        
        public Organisation(string name, string description, string[]? tags)
        {
            Name = name;
            Description = description;
            Tags = tags;
        }

        public void AddProject(Project project) =>
            _projects.Add(project ?? throw new ArgumentNullException(nameof(project)));
        
        public void AddUser(User user) =>
            _organisationUsers.Add(new OrganisationUser(Id,
                user ?? throw new ArgumentNullException(nameof(user))));

        public void RemoveUser(User user) =>
            _organisationUsers.RemoveAll(x => x.UserId ==
                                              (user ?? throw new ArgumentNullException(nameof(user))).Id);

        public override string ToString() => Name;
    }
}