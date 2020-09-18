using System;
using System.Collections.Generic;
using MediatR;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Admin.Organisations
{
    public class ListOrganisationsQuery : IRequest<List<Organisation>>
    {
        public Guid? UserId { get; set; }

        public ListOrganisationsQuery(Guid? userId)
        {
            UserId = userId;
        }

        public ListOrganisationsQuery()
        {
            
        }
    }
}