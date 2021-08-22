using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Domain.Repositories;

namespace PingAI.DialogManagementService.Application.Admin.Users
{
    public class ListUsersQueryHandler : IRequestHandler<ListUsersQuery, List<User>>
    {
        private readonly IUserRepository _userRepository;

        public ListUsersQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        
        public async Task<List<User>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.ListAll();
            return users;
        }
    }
}