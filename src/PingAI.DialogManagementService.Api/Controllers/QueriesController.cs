using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PingAI.DialogManagementService.Api.Models.Queries;
using PingAI.DialogManagementService.Application.Queries.CreateQuery;
using PingAI.DialogManagementService.Application.Queries.GetQuery;
using PingAI.DialogManagementService.Application.Queries.ListQueries;
using PingAI.DialogManagementService.Application.Queries.UpdateQuery;
using PingAI.DialogManagementService.Domain.ErrorHandling;
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
        
        [HttpGet]
        public async Task<ActionResult<List<QueryListItemDto>>> GetQuery([FromQuery] Guid? projectId)
        {
            if (!projectId.HasValue)
            {
                throw new BadRequestException($"{nameof(projectId)} must be provided");
            }
            var query = new ListQueriesQuery(projectId.Value);
            var queries = await _mediator.Send(query);
            return new ListQueriesResponse(queries.Select(q => new QueryListItemDto(q)));
        }

        [HttpGet("{queryId}")]
        public async Task<ActionResult<QueryDto>> GetQuery([FromRoute] Guid queryId)
        {
            var query = new GetQueryQuery(queryId);
            var result = await _mediator.Send(query);
            return new QueryDto(result);
        }

        [HttpPost]
        public async Task<ActionResult<CreateQueryResponse>> CreateQuery([FromBody] CreateQueryRequest request)
        {
            var projectId = Guid.Parse(request.ProjectId);
            var intent = new Intent(request.Intent.Name, projectId, IntentType.STANDARD);
            var response = new Response(projectId, ResponseType.RTE, request.Response.Order);
            var query = await _mediator.Send(new CreateQueryCommand(
                request.Name, Guid.Parse(request.ProjectId),
                new Expression[0], request.Description ?? request.Name, request.Tags,
                request.DisplayOrder, null, intent, null, response, request.Response.RteText));
            return new CreateQueryResponse(query);
        }

        [HttpPut("{queryId}")]
        public async Task<ActionResult<UpdateQueryResponse>> UpdateQuery(
            [FromRoute] Guid queryId,
            [FromBody] UpdateQueryRequest request)
        {
            var intent = new Intent(request.Intent.Name, IntentType.STANDARD);
            var response = new Response(ResponseType.RTE, request.Response.Order);
            var query = await _mediator.Send(new UpdateQueryCommand(
                queryId,
                request.Name, 
                new Expression[0], request.Description ?? request.Name, request.Tags,
                request.DisplayOrder, null, intent, null, response, request.Response.RteText));
            return new UpdateQueryResponse(query);
        }
    }
}