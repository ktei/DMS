using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Domain.Repositories;

namespace PingAI.DialogManagementService.Application.Admin.Users
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, User>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _uow;

        public CreateUserCommandHandler(IUserRepository userRepository, IUnitOfWork uow)
        {
            _userRepository = userRepository;
            _uow = uow;
        }
        
        public async Task<User> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            // var existingUser = await _userRepository.GetUserByName(request.Name);
            // if (existingUser != null)
            //     throw new BadRequestException($"User {request.Name} already exists");
            var existingUser = await _userRepository.FindByAuth0Id(request.Auth0Id);
            if (existingUser != null)
                throw new BadRequestException($"User {request.Name} already exists", ErrorCodes.UserExists);

            var user = new User(request.Name, request.Auth0Id);
            user = await _userRepository.Add(user);
            await _uow.SaveChanges();

            return user;
        }
    }
}