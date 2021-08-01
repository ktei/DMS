using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.Runtime.Internal.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PingAI.DialogManagementService.Api.Models.Projects;
using PingAI.DialogManagementService.Application.Projects.GetIntegration;
using PingAI.DialogManagementService.Application.Projects.GetProject;
using PingAI.DialogManagementService.Application.Projects.ListProjects;
using PingAI.DialogManagementService.Application.Projects.PrepareUpload;
using PingAI.DialogManagementService.Application.Projects.PublishProject;
using PingAI.DialogManagementService.Application.Projects.UpdateEnquiries;
using PingAI.DialogManagementService.Application.Projects.UpdateProject;
using PingAI.DialogManagementService.Domain.Model;

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
        
        [HttpGet("{projectId}")]
        public async Task<ActionResult<ProjectDto>> GetProject([FromRoute] Guid projectId)
        {
            var project = await _mediator.Send(new GetProjectQuery(projectId));
            return new ProjectDto(project);
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

        [HttpPut("{projectId}")]
        public async Task<ActionResult<ProjectDto>> UpdateProject([FromRoute] Guid projectId,
            [FromBody] UpdateProjectRequest request)
        {
            var businessTimeStartUtc = string.IsNullOrEmpty(request.BusinessTimeStart)
                ? null
                : ProjectDto.TryConvertStringToUtc(request.BusinessTimeStart);
            var businessTimeEndUtc = string.IsNullOrEmpty(request.BusinessTimeEnd)
                ? null
                : ProjectDto.TryConvertStringToUtc(request.BusinessTimeEnd);

            var greetingResponses = new List<Response>();
            if (request.GreetingMessage != null)
            {
                var r = new Response(projectId, Resolution.Factory.RteText(request.GreetingMessage), ResponseType.RTE,
                    0);
                greetingResponses.Add(r);
            }

            if (request.QuickReplies != null)
            {
                var responseOrder = 1;
                foreach (var quickReply in request.QuickReplies)
                {
                    var r = new Response(projectId, Resolution.Factory.RteText(quickReply), ResponseType.QUICK_REPLY,
                        responseOrder++);
                    greetingResponses.Add(r);
                }
            }

            var project = await _mediator.Send(new UpdateProjectCommand(projectId,
                request.WidgetTitle, request.WidgetColor,
                request.WidgetDescription, request.FallbackMessage,
                greetingResponses, request.Domains,
                businessTimeStartUtc, businessTimeEndUtc, request.BusinessEmail));
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

        [HttpPost("{projectId}/upload")]
        public async Task<ActionResult<UploadResponse>> Upload([FromRoute] Guid projectId,
            [FromBody] UploadRequest request)
        {
            var result = await _mediator.Send(new PrepareUploadCommand(projectId, request.ContentType,
                request.FileName));
            return new UploadResponse(result.UploadUrl, result.PublicUrl);
        }
    }
}

