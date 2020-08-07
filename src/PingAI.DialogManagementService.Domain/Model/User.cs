using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class User : IHaveTimestamps
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Auth0Id { get; private set; }
        
        private readonly List<OrganisationUser> _organisationUsers = new List<OrganisationUser>();
        public IReadOnlyList<OrganisationUser> OrganisationUsers => _organisationUsers.ToImmutableList();
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public User(Guid id, string name, string auth0Id)
        {
            Id = id;
            Name = name;
            Auth0Id = auth0Id;
        }

        public override string ToString() => Name;
    }
}