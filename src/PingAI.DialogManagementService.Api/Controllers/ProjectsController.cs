using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PingAI.DialogManagementService.Api.Models.Projects;
using PingAI.DialogManagementService.Application.Projects.UpdateProject;

namespace PingAI.DialogManagementService.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProjectsController(IMediator mediator)
        {
            _mediator = mediator;
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