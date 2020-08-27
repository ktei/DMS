using System;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class SlackWorkspace : DomainEntity, IHaveTimestamps
    {
        public Guid Id { get; private set; }
        public Guid ProjectId { get; private set; }
        public Project? Project { get; private set; }
        public string OAuthAccessToken { get; private set; }
        public string WebhookUrl { get; private set; }

        public SlackWorkspace(Guid projectId, string oAuthAccessToken, string webhookUrl)
        {
            ProjectId = projectId;
            OAuthAccessToken = oAuthAccessToken;
            WebhookUrl = webhookUrl;
        }
    }
}