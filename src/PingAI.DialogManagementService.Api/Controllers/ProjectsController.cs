using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PingAI.DialogManagementService.Api.Models.Projects;
using PingAI.DialogManagementService.Application.Projects.GetIntegration;
using PingAI.DialogManagementService.Application.Projects.GetProject;
using PingAI.DialogManagementService.Application.Projects.ListProjects;
using PingAI.DialogManagementService.Application.Projects.PublishProject;
using PingAI.DialogManagementService.Application.Projects.UpdateEnquiries;
using PingAI.DialogManagementService.Application.Projects.UpdateProject;

namespace PingAI.DialogManagementService.Api.Controllers
{
    [ApiVersion("1")]
    [Authorize]
    public class ProjectsController : ApiControllerBase
    {
        private readonly IMediator _mediator;

        public ProjectsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProjectListItemDto>>> ListProjects()
        {
            var projects = await _mediator.Send(new ListProjectsQuery());
            return projects.Select(p => new ProjectListItemDto(p)).ToList();
        }

        [HttpGet("{projectId}/integration")]
        public async Task<ActionResult<IntegrationDto>> GetIntegration([FromRoute] Guid projectId)
        {
            var integration = await _mediator.Send(new GetIntegrationQuery(projectId));
            var connections = new List<string>();
            if (integration.SlackWorkspace != null)
                connections.Add("slack");
            return new IntegrationDto(connections.ToArray());
        }
        
        [HttpGet("{projectId}")]
        public async Task<ActionResult<ProjectDto>> GetProject([FromRoute] Guid projectId)
        {
            var project = await _mediator.Send(new GetProjectQuery(projectId));
            return new ProjectDto(project);
        }

        [HttpPut("{projectId}")]
        public async Task<ActionResult<ProjectDto>> UpdateProject([FromRoute] Guid projectId,
            [FromBody] UpdateProjectRequest request)
        {
            var project = await _mediator.Send(new UpdateProjectCommand(projectId,
                request.WidgetTitle, request.WidgetColor,
                request.WidgetDescription, request.FallbackMessage,
                request.GreetingMessage, request.Domains));
            return new ProjectDto(project);
        }

        [HttpPut("{projectId}/enquiries")]
        public async Task<ActionResult<UpdateEnquiriesResponse>> UpdateEnquiries([FromRoute] Guid projectId,
            [FromBody] UpdateEnquiriesRequest request)
        {
            var project = await _mediator.Send(new UpdateEnquiriesCommand(projectId, request.Enquiries));
            return new UpdateEnquiriesResponse(project);
        }

        [HttpPost("{projectId}/publish")]
        public async Task<ActionResult<Guid>> Publish([FromRoute] Guid projectId)
        {
            var projectPublished = await _mediator.Send(new PublishProjectCommand(projectId));
            return projectPublished.Id;
        }
    }
}