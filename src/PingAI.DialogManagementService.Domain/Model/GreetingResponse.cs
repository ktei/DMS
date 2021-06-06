using System;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class GreetingResponse : DomainEntity, IHaveTimestamps
    {
        public Guid Id { get; private set; }
        public Guid ProjectId { get; private set; }
        public Project? Project { get; private set; }
        public Guid ResponseId { get; private set; }
        public Response? Response { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private GreetingResponse()
        {
            
        }

        public GreetingResponse(Guid projectId, Response response)
        {
            ProjectId = projectId;
            Response = response;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
        
        public override string ToString() => $"Project:{ProjectId}-Response:{ResponseId}";
    }
}
