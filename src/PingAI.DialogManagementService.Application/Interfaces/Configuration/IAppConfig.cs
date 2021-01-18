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
        string AdminPortalClientId { get; }
        
        /// <summary>
        /// OAuth client_id of ChatbotRuntime
        /// </summary>
        string ChatbotRuntimeClientId { get; }
        
        string Auth0RulesClientId { get; }
        
        string BucketName { get; }
        
        string PublicBaseUrl { get; }
    }
}