using System;
using MediatR;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Admin.Organisations
{
    public class CreateOrganisationCommand : IRequest<Organisation>
    {
        public string Name { get; set; }
        public string? Auth0UserId { get; set; }

        public CreateOrganisationCommand(string name, string? auth0UserId)
        {
            Name = name;
            Auth0UserId = auth0UserId;
        }

        public CreateOrganisationCommand()
        {
            
        }
    }
}