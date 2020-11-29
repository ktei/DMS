using MediatR;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Admin.Organisations
{
    public class CreateOrganisationCommand : IRequest<Organisation>
    {
        public string Name { get; set; }
        public string? Auth0UserId { get; set; }
        public string? Description { get; set; }

        public CreateOrganisationCommand(string name, string? auth0UserId, string? description)
        {
            Name = name;
            Auth0UserId = auth0UserId;
            Description = description;
        }

        public CreateOrganisationCommand()
        {
            
        }
    }
}