using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using PingAI.DialogManagementService.Api.Authorization.Requirements;
using PingAI.DialogManagementService.Api.Authorization.Utils;
using PingAI.DialogManagementService.Application.Interfaces.Configuration;

namespace PingAI.DialogManagementService.Api.Authorization.Handlers
{
    public class AdministratorOnlyAuthorizationHandler :
        AuthorizationHandler<AdministratorOnlyRequirement>

    {
        private readonly IAppConfig _appConfig;

        public AdministratorOnlyAuthorizationHandler(IAppConfig appConfig)
        {
            _appConfig = appConfig;
        }
        
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            AdministratorOnlyRequirement requirement)
        {
            if (context.User.IsClient(_appConfig.AdminPortalClientId) ||
                context.User.IsClient(_appConfig.ChatbotRuntimeClientId) ||
                context.User.HasAdminScope())
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            
            context.Fail();
            return Task.CompletedTask;
        }
    }
}