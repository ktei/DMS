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

        public GreetingResponse(Guid projectId, Guid responseId)
        {
            ProjectId = projectId;
            ResponseId = responseId;
        }

        public GreetingResponse(Response response)
        {
            Response = response;
        }
        
        public override string ToString() => $"Project:{ProjectId}-Response:{ResponseId}";
    }
}
