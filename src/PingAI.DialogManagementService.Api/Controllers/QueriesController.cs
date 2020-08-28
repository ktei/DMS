using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PingAI.DialogManagementService.Api.Models.Intents;
using PingAI.DialogManagementService.Api.Models.Queries;
using PingAI.DialogManagementService.Application.Queries.CreateQuery;
using PingAI.DialogManagementService.Application.Queries.DeleteQuery;
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
        public async Task<ActionResult<List<QueryListItemDto>>> ListQueries([FromQuery] Guid? projectId,
            [FromQuery] string? queryType)
        {
            if (!projectId.HasValue)
            {
                throw new BadRequestException($"{nameof(projectId)} must be provided");
            }

            static bool CheckQueryType(string? t)
            {
                switch (t)
                {
                    case null:
                    case QueryTypes.Faq:
                    case QueryTypes.Handover:
                        return true;
                    default:
                        return false;
                }
            }
            
            if (!CheckQueryType(queryType))
                throw new BadRequestException($"{nameof(queryType)} is invalid. Accepted types are " +
                                              $"{QueryTypes.Faq}, {QueryTypes.Handover}");
            
            var query = new ListQueriesQuery(projectId.Value, queryType);
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
            var phraseParts = FlattenPhraseParts(projectId, Guid.Empty, request.Intent.PhraseParts);
            var intent = new Intent(request.Intent.Name, projectId, IntentType.STANDARD, phraseParts);
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
            var phraseParts = FlattenPhraseParts(Guid.Empty, Guid.Empty, request.Intent.PhraseParts);
            var intent = new Intent(request.Intent.Name, Guid.Empty, IntentType.STANDARD, phraseParts);
            var response = new Response(ResponseType.RTE, request.Response.Order);
            var query = await _mediator.Send(new UpdateQueryCommand(
                queryId,
                request.Name, 
                new Expression[0], request.Description ?? request.Name, request.Tags,
                request.DisplayOrder, null, intent, null, response, request.Response.RteText));
            return new UpdateQueryResponse(query);
        }

        private static IEnumerable<PhrasePart> FlattenPhraseParts(Guid projectId, Guid intentId,
            CreatePhrasePartDto[][] partsGroups)
        {
            foreach (var parts in partsGroups)
            {
                var phraseId = Guid.NewGuid();
                foreach (var part in parts)
                {
                    yield return new PhrasePart(intentId, phraseId, part.Position, part.Text,
                        part.Value, Enum.Parse<PhrasePartType>(part.Type),
                        string.IsNullOrEmpty(part.EntityName) ? null : new EntityName(part.EntityName, projectId, true),
                        string.IsNullOrEmpty(part.EntityTypeId) ? default(Guid?) : Guid.Parse(part.EntityTypeId));
                }
            }
        }

        [HttpDelete("{queryId}")]
        public async Task<IActionResult> DeleteQuery([FromRoute] Guid queryId)
        {
            await _mediator.Send(new DeleteQueryCommand(queryId));
            return Ok();
        }
    }
}