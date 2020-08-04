using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PingAI.DialogManagementService.Api.Models.Projects;
using PingAI.DialogManagementService.Application.Projects.GetProject;
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
        
        [HttpGet("{projectId}")]
        public async Task<ActionResult<GetProjectResponse>> GetProject([FromRoute] Guid projectId)
        {
            var project = await _mediator.Send(new GetProjectQuery(projectId));
            return new GetProjectResponse(project);
        }

        [HttpPut("{projectId}")]
        public async Task<ActionResult<UpdateProjectResponse>> UpdateProject([FromRoute] Guid projectId,
            [FromBody] UpdateProjectRequest request)
        {
            var project = await _mediator.Send(new UpdateProjectCommand(projectId,
                request.WidgetTitle, request.WidgetColor,
                request.WidgetDescription, request.FallbackMessage,
                request.GreetingMessage, request.Enquiries));
            return new UpdateProjectResponse(project);
        }
    }
}