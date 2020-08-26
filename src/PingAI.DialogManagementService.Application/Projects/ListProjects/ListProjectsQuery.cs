using System.Collections.Generic;
using MediatR;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Projects.ListProjects
{
    public class ListProjectsQuery : IRequest<List<Project>>
    {
        public ListProjectsQuery()
        {
        }
    }
}