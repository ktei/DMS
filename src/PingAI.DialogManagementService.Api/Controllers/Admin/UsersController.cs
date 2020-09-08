using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PingAI.DialogManagementService.Api.Models.Users;
using PingAI.DialogManagementService.Application.Admin.Users;

namespace PingAI.DialogManagementService.Api.Controllers.Admin
{
    [ApiVersion("1")]
    public class UsersController : AdminControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpPost]
        public async Task<UserDto> CreateUser([FromBody] CreateUserRequest request)
        {
            var user = await _mediator.Send(new CreateUserCommand(request.Name, request.Auth0Id));
            return new UserDto(user);
        }

        [HttpGet]
        public async Task<List<UserListItemDto>> ListUsers()
        {
            var users = await _mediator.Send(new ListUsersQuery());
            return users.Select(u => new UserListItemDto(u)).ToList();
        }
    }
}