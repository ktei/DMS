using System;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class OrganisationUser : IHaveTimestamps
    {
        public Guid Id { get; private set; }
        public Guid OrganisationId { get; private set; }
        public Organisation? Organisation { get; private set; }
        public Guid UserId { get; private set; }
        public User? User { get; private set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        public OrganisationUser(Guid id, Guid organisationId, Guid userId)
        {
            Id = id;
            OrganisationId = organisationId;
            UserId = userId;
        }
    }
}