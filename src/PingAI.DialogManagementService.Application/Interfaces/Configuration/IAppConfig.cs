namespace PingAI.DialogManagementService.Application.Interfaces.Configuration
{
    public interface IAppConfig
    {
        /// <summary>
        /// Prefix for the key of every item to be cached
        /// </summary>
        string GlobalCachePrefix { get; }
        
        /// <summary>
        /// OAuth client_id of DialogManagementAdminClient
        /// </summary>
        string AdminClientId { get; }
    }
}