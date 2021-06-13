using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using PingAI.DialogManagementService.Api.Authorization.Requirements;
using IAuthorizationService = PingAI.DialogManagementService.Application.Interfaces.Services.Security.IAuthorizationService;

namespace PingAI.DialogManagementService.Api.Authorization.Services
{
    public class AuthorizationService : Application.Interfaces.Services.Security.IAuthorizationService
    {
        private readonly Microsoft.AspNetCore.Authorization.IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthorizationService(Microsoft.AspNetCore.Authorization.IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor)
        {
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> UserCanReadProject(Guid projectId)
        {
            var authResult =
                await _authorizationService.AuthorizeAsync(_httpContextAccessor.HttpContext.User, projectId,
                    Operations.Read);
            return authResult.Succeeded;
        }

        public async Task<bool> UserCanWriteProject(Guid projectId)
        {
            var authResult =
                await _authorizationService.AuthorizeAsync(_httpContextAccessor.HttpContext.User, projectId,
                    Operations.Write);
            return authResult.Succeeded;
        }

        public async Task<bool> HasAdminPrivilege()
        {
            var authResult =
                await _authorizationService.AuthorizeAsync(_httpContextAccessor.HttpContext.User, null,
                    new[] {new AdministratorOnlyRequirement()});
            return authResult.Succeeded;
        }
    }
}