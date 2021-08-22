using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using PingAI.DialogManagementService.Api.Authorization.Utils;
using PingAI.DialogManagementService.Application.Interfaces.Configuration;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Domain.Repositories;

namespace PingAI.DialogManagementService.Api.Authorization.Handlers
{
    public class ProjectAuthorizationHandler :
        AuthorizationHandler<OperationAuthorizationRequirement, Guid>
    {
        private readonly IUserRepository _userRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IAppConfig _appConfig;

        public ProjectAuthorizationHandler(IUserRepository userRepository, IAppConfig appConfig,
            IProjectRepository projectRepository)
        {
            _userRepository = userRepository;
            _appConfig = appConfig;
            _projectRepository = projectRepository;
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

            if (context.User.HasAdminScope() ||
                context.User.IsClient(_appConfig.AdminPortalClientId) ||
                context.User.IsClient(_appConfig.ChatbotRuntimeClientId) ||
                context.User.IsClient(_appConfig.Auth0RulesClientId))
            {
                context.Succeed(requirement);
                return;
            }

            var user = await _userRepository.FindByAuth0Id(context.User.Identity.Name!);
            if (user == null)
            {
                context.Fail();
                return;
            }

            // get IDs of projects this user belongs to
            var projects = await Task.WhenAll(user.Organisations.Select(o => o.Id)
                .Select(_projectRepository.ListByOrganisationId));
            var projectIds = projects.SelectMany(p => p).Select(x => x.Id);
            if (projectIds.Contains(resource))
            {
                context.Succeed(requirement);
                return;
            }

            context.Fail();
        }
    }
}

