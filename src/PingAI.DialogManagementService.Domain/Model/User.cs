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
        private DateTime _createdAt;
        public DateTime CreatedAt
        {
            get => DateTime.SpecifyKind(_createdAt, DateTimeKind.Utc);
            private set => _createdAt = value;
        }
        private readonly List<OrganisationUser> _organisationUsers;
        public IReadOnlyList<OrganisationUser> OrganisationUsers => _organisationUsers.ToImmutableList();

        public IReadOnlyList<Guid> OrganisationIds => GetOrganisationIds();

        public const int MaxNameLength = 250;
        
        public User(string name, string auth0Id)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"{nameof(name)} cannot be empty");
            
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

        public override string ToString() => Name;
    }
}