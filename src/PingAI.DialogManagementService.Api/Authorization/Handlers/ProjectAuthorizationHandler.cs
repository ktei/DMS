using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using PingAI.DialogManagementService.Api.Authorization.Utils;
using PingAI.DialogManagementService.Application.Interfaces.Configuration;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;

namespace PingAI.DialogManagementService.Api.Authorization.Handlers
{
    public class ProjectAuthorizationHandler :
        AuthorizationHandler<OperationAuthorizationRequirement, Guid>
    {
        private readonly IUserRepository _userRepository;
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IAppConfig _appConfig;

        public ProjectAuthorizationHandler(IUserRepository userRepository,
            IOrganisationRepository organisationRepository, IAppConfig appConfig)
        {
            _userRepository = userRepository;
            _organisationRepository = organisationRepository;
            _appConfig = appConfig;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            Guid resource)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                context.Fail();
                return;
            }
            
            if (context.User.HasAdminScope() || context.User.IsClient(_appConfig.AdminClientId))
            {
                context.Succeed(requirement);
                return;
            }
            
            var user = await _userRepository.GetUserByAuth0Id(context.User.Identity.Name!);
            if (user == null)
            {
                context.Fail();
                return;
            }
            
            // get IDs of organisations this user belongs to
            var organisationIds = user.OrganisationUsers.Select(o => o.OrganisationId).ToArray();
            var organisations = await _organisationRepository.GetOrganisationsByIds(organisationIds);
            
            // get IDs of projects this user belongs to
            var projectIds = organisations
                .SelectMany(o => o.Projects)
                .Select(p => p.Id);

            if (projectIds.Contains(resource))
            {
                context.Succeed(requirement);
                return;
            }
            context.Fail();
        }
    }
}