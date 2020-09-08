using System;
using MediatR;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Admin.Organisations
{
    public class CreateOrganisationCommand : IRequest<Organisation>
    {
        public string Name { get; set; }
        public Guid? AdminUserId { get; set; }

        public CreateOrganisationCommand(string name, Guid? adminUserId)
        {
            Name = name;
            AdminUserId = adminUserId;
        }

        public CreateOrganisationCommand()
        {
            
        }
    }
}