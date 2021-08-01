using System;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class GreetingResponse : DomainEntity
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

        public GreetingResponse(Project project, Response response)
        {
            Id = Guid.NewGuid();
            Project = project;
            Response = response;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
        
        public override string ToString() => $"Project:{ProjectId}-Response:{ResponseId}";
    }
}
