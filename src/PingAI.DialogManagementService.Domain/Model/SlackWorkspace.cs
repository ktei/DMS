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

        public void UpdateSlackConfig(string oauthAccessToken, string webhookUrl)
        {
            if (string.IsNullOrEmpty(oauthAccessToken))
                throw new ArgumentException(nameof(oauthAccessToken));
            if (string.IsNullOrEmpty(webhookUrl))
                throw new ArgumentException(nameof(webhookUrl));
            OAuthAccessToken = oauthAccessToken;
            WebhookUrl = webhookUrl;
        }
    }
}