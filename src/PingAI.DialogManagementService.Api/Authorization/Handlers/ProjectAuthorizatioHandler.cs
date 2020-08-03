using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Authorization.Handlers
{
    public class ProjectAuthorizationHandler :
        AuthorizationHandler<OperationAuthorizationRequirement, Project>
    {
        private readonly IUserRepository _userRepository;
        private readonly IOrganisationRepository _organisationRepository;

        private static readonly Guid[] SuperAdmins = new[]
        {
            "3ec1b42a-aada-4487-8ac1-ee2c5ef4cc7f", // rui
            "6ace6d50-05b3-46b8-9fdd-25e94e778d3e", // robert
            "a8e2ceb2-6564-4121-9fae-4dd9ee861d8c", // ping
            "ef8ab59a-fad8-490a-ab2a-1cb70d6b53bf" // steve
        }.Select(Guid.Parse).ToArray();

        public ProjectAuthorizationHandler(IUserRepository userRepository, IOrganisationRepository organisationRepository)
        {
            _userRepository = userRepository;
            _organisationRepository = organisationRepository;
        }
        
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            Project resource)
        {
            var user = await _userRepository.GetUserByAut0Id(context.User.Identity.Name);
            if (user == null)
            {
                context.Fail();
                return;
            }

            if (SuperAdmins.Contains(user.Id))
            {
                context.Succeed(requirement);
                return;
            }

            // get IDs of organisations this user belongs to
            var organisationIds = user.OrganisationUsers.Select(o => o.OrganisationId).ToArray();
            var organisations = await _organisationRepository.GetOrganisationsByIds(organisationIds);
            
            // get IDs of projects this user belongs to
            var projectIds = organisations
                .SelectMany(o => o.Projects)
                .Select(p => p.Id);

            if (projectIds.Contains(resource.Id))
            {
                context.Succeed(requirement);
                return;
            }
            context.Fail();
        }
    }
}