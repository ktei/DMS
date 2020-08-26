using System;
using System.Collections.Generic;
using MediatR;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Projects.Admin
{
    public class ListProjectsQuery : IRequest<List<Project>>
    {
        public Guid OrganisationId { get; set; }

        public ListProjectsQuery(Guid organisationId)
        {
            OrganisationId = organisationId;
        }

        public ListProjectsQuery()
        {
            
        }
    }
}