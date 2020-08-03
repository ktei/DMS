using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using PingAI.DialogManagementService.Api.Authorization.Requirements;
using PingAI.DialogManagementService.Application.Interfaces.Services;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Authorization.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor)
        {
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> UserCanWriteProject(Project project)
        {
            var authResult =
                await _authorizationService.AuthorizeAsync(_httpContextAccessor.HttpContext.User, project,
                    Operations.Write);
            return authResult.Succeeded;
        }
    }
}