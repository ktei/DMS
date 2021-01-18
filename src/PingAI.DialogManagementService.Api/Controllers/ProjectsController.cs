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
            var businessTimeStartUtc = string.IsNullOrEmpty(request.BusinessTimeStart)
                ? null
                : ProjectDto.TryConvertStringToUtc(request.BusinessTimeStart);
            var businessTimeEndUtc = string.IsNullOrEmpty(request.BusinessTimeEnd)
                ? null
                : ProjectDto.TryConvertStringToUtc(request.BusinessTimeEnd);
            var project = await _mediator.Send(new UpdateProjectCommand(projectId,
                request.WidgetTitle, request.WidgetColor,
                request.WidgetDescription, request.FallbackMessage,
                request.GreetingMessage, request.Domains,
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
            // UploadObject(result.UploadUrl);
            return new UploadResponse(result.UploadUrl, result.PublicUrl);
        }
        
        private static void UploadObject(string url)
        {
            HttpWebRequest httpRequest = WebRequest.Create(url) as HttpWebRequest;
            httpRequest.Method = "PUT";
            using (Stream dataStream = httpRequest.GetRequestStream())
            {
                var buffer = new byte[8000];
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "SS architecture.png");
                using (FileStream fileStream = new FileStream(path,
                    FileMode.Open, FileAccess.Read))
                {
                    int bytesRead = 0;
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        dataStream.Write(buffer, 0, bytesRead);
                    }
                }
            }
            HttpWebResponse response = httpRequest.GetResponse() as HttpWebResponse;
        }
    }
}
