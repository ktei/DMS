using System.Collections.Generic;
using MediatR;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Admin.Users
{
    public class ListUsersQuery : IRequest<List<User>>
    {
        
    }
}