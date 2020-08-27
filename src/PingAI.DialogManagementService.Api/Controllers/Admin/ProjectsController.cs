using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PingAI.DialogManagementService.Api.Models.Projects;
using PingAI.DialogManagementService.Application.Admin.Projects;
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
            if (!organisationId.HasValue)
            {
                throw new BadRequestException($"Must provide {nameof(organisationId)}");
            }
            var projects = await _mediator.Send(new ListProjectsQuery(organisationId.Value));
            return projects.Select(p => new ProjectListItemDto(p.Id.ToString(), p.Name)).ToList();
        }
    }
}