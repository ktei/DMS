using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PingAI.DialogManagementService.Api.Models.Queries;
using PingAI.DialogManagementService.Application.Queries.CreateQuery;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Controllers
{
    [ApiController]
    [Route("dms/api/v{version:apiVersion}/[controller]")]
    public class QueriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public QueriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<CreateQueryResponse>> CreateQuery([FromBody] CreateQueryRequest request)
        {
            var projectId = Guid.Parse(request.ProjectId);
            var intent = new Intent(request.Intent.Name, projectId, IntentType.STANDARD);
            var response = new Response(new ResolutionPart[0], projectId, ResponseType.RTE, request.Response.Order);
            var query = await _mediator.Send(new CreateQueryCommand(
                request.Name, Guid.Parse(request.ProjectId),
                new Expression[0], request.Description ?? request.Name, request.Tags,
                request.DisplayOrder, null, intent, null, response, request.Response.RteText));
            return new CreateQueryResponse(query);
        }
    }
}