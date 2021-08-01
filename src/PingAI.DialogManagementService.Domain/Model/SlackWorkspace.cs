using System;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class SlackWorkspace : DomainEntity
    {
        public Guid Id { get; private set; }
        public Guid ProjectId { get; private set; }
        public Project? Project { get; private set; }
        public string OAuthAccessToken { get; private set; }
        public string WebhookUrl { get; private set; }
        public string TeamId { get; private set; }

        public SlackWorkspace(Guid projectId, string oAuthAccessToken, string webhookUrl,
            string teamId)
        {
            Id = Guid.NewGuid();
            ProjectId = projectId;
            OAuthAccessToken = oAuthAccessToken;
            WebhookUrl = webhookUrl;
            TeamId = teamId;
        }

        public void UpdateSlackConfig(string oauthAccessToken, string webhookUrl,
            string teamId)
        {
            if (string.IsNullOrEmpty(oauthAccessToken))
                throw new ArgumentException(nameof(oauthAccessToken));
            if (string.IsNullOrEmpty(webhookUrl))
                throw new ArgumentException(nameof(webhookUrl));
            if (string.IsNullOrEmpty(teamId))
                throw new ArgumentException(nameof(teamId));
            OAuthAccessToken = oauthAccessToken;
            WebhookUrl = webhookUrl;
            TeamId = teamId;
        }
    }
}