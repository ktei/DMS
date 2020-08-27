namespace PingAI.DialogManagementService.Infrastructure.Configuration
{
    public interface IConfigurationManager
    {
        string SlackClientId { get; }
        string SlackClientSecret { get; }
    }
}