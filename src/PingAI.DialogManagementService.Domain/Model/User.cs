using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class User : DomainEntity
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Auth0Id { get; private set; }
        public DateTime CreatedAt { get; private set; }
        private readonly List<Organisation> _organisations;
        public IReadOnlyList<Organisation> Organisations => _organisations.ToImmutableList();

        public const int MaxNameLength = 250;
        
        public User(string name, string auth0Id)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"{nameof(name)} cannot be empty");
            
            Name = name;
            Auth0Id = auth0Id;
            _organisations = new List<Organisation>();
        }
        
        public override string ToString() => Name;
    }
}
