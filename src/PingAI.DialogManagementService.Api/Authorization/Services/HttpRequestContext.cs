using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Authorization.Services
{
    public class HttpRequestContext : IRequestContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;

        public HttpRequestContext(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
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
            var user = await _userRepository.GetUserByAut0Id(_httpContextAccessor.HttpContext.User.Identity.Name);
            if (user == null)
                throw new UnauthorizedException("Unauthenticated request");
            return user!;
        }
    }
}