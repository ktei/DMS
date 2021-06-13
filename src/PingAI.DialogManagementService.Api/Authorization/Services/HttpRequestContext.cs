using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PingAI.DialogManagementService.Application.Interfaces.Services.Security;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Domain.Repositories;

namespace PingAI.DialogManagementService.Api.Authorization.Services
{
    public class HttpIdentityContext : IIdentityContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;

        public HttpIdentityContext(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Get user who made current request 
        /// </summary>
        /// <returns>The user</returns>
        /// <exception cref="UnauthorizedException">Thrown when request is not authenticated or user is not found</exception>
        public async Task<User> GetUser()
        {
            if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                throw new UnauthorizedException("Unauthenticated request");
            var user = await _userRepository.GetUserByAuth0Id(_httpContextAccessor.HttpContext.User.Identity.Name!);
            if (user == null)
                throw new UnauthorizedException("Unauthenticated request");
            return user;
        }
    }
}
