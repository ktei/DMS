using MediatR;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Admin.Users
{
    public class CreateUserCommand : IRequest<User>
    {
        public string Name { get; set; }
        public string Auth0Id { get; set; }

        public CreateUserCommand(string name, string auth0Id)
        {
            Name = name;
            Auth0Id = auth0Id;
        }

        public CreateUserCommand()
        {
            
        }
    }
}