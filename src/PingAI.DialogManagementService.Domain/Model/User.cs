using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class User : DomainEntity, IHaveTimestamps
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Auth0Id { get; private set; }

        private readonly List<OrganisationUser> _organisationUsers;
        public IReadOnlyList<OrganisationUser> OrganisationUsers => _organisationUsers.ToImmutableList();

        public IReadOnlyList<Guid> OrganisationIds => GetOrganisationIds();
        
        public User(string name, string auth0Id)
        {
            Name = name;
            Auth0Id = auth0Id;
            _organisationUsers = new List<OrganisationUser>();
        }
        
        private IReadOnlyList<Guid> GetOrganisationIds()
        {
            if (_organisationUsers == null)
                throw new InvalidOperationException($"Load {nameof(OrganisationUsers)} first");
            return _organisationUsers.Select(x => x.OrganisationId!).ToImmutableList();
        }

        public bool CanAccessOrganisation(Guid organisationId) => OrganisationIds.Contains(organisationId);

        public override string ToString() => Name;
    }
}