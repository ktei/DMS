using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PingAI.DialogManagementService.Api.Models.Projects;
using PingAI.DialogManagementService.Application.Admin.Projects;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Domain.ErrorHandling;

namespace PingAI.DialogManagementService.Api.Controllers.Admin
{
    [ApiVersion("1")]
    public class ProjectsController : AdminControllerBase
    {
        private readonly IMediator _mediator;

        public ProjectsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpGet]
        public async Task<ActionResult<List<ProjectListItemDto>>> ListProjects([FromQuery] Guid? organisationId)
        {
            var projects = await _mediator.Send(new ListProjectsQuery(organisationId));
            return projects.Select(p => new ProjectListItemDto(p)).ToList();
        }

        // TODO: fix me - use mediatR
        [HttpGet("{projectId}")]
        public async Task<ActionResult<ProjectDto>> GetProject([FromRoute] Guid projectId,
            [FromServices] IProjectRepository projectRepository)
        {
            var project = await projectRepository.GetProjectById(projectId);
            if (project == null)
            {
                throw new NotFoundException($"Project {projectId} does not exist");
            }

            return new ProjectDto(project);
        }

        [HttpGet("{designTimeProjectId}/runtime")]
        public async Task<ActionResult<ProjectDto>> GetRuntimeProjectByDesignTimeProjectId(
            [FromRoute] Guid designTimeProjectId)
        {
            var project = await _mediator.Send(new GetRuntimeProjectQuery(designTimeProjectId));
            return new ProjectDto(project);
        }
    }
}