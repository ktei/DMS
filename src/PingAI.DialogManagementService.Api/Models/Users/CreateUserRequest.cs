using MediatR;

namespace PingAI.DialogManagementService.Api.Models.Users
{
    public class CreateUserRequest
    {
        public string Name { get; set; }
        public string Auth0Id { get; set; }
    }
}