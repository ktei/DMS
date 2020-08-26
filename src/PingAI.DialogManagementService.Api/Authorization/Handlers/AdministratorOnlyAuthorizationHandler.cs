using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using PingAI.DialogManagementService.Api.Authorization.Requirements;
using PingAI.DialogManagementService.Api.Authorization.Utils;

namespace PingAI.DialogManagementService.Api.Authorization.Handlers
{
    public class AdministratorOnlyAuthorizationHandler :
        AuthorizationHandler<AdministratorOnlyRequirement>

    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            AdministratorOnlyRequirement requirement)
        {
            if (!context.User.HasScope("admin"))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}