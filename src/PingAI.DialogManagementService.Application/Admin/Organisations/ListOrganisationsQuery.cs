using System;
using System.Collections.Generic;
using MediatR;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Admin.Organisations
{
    public class ListOrganisationsQuery : IRequest<List<Organisation>>
    {
        public string? Auth0UserId { get; set; }

        public ListOrganisationsQuery(string? auth0UserId)
        {
            Auth0UserId = auth0UserId;
        }

        public ListOrganisationsQuery()
        {
            
        }
    }
}