using System;
using System.Collections.Generic;
using MediatR;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Admin.Projects
{
    public class ListProjectsQuery : IRequest<List<Project>>
    {
        public Guid? OrganisationId { get; set; }

        public ListProjectsQuery(Guid? organisationId)
        {
            OrganisationId = organisationId;
        }

        public ListProjectsQuery()
        {
            
        }
    }
}