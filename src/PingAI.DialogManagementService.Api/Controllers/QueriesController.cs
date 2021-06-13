using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PingAI.DialogManagementService.Api.Models.Intents;
using PingAI.DialogManagementService.Api.Models.Queries;
using PingAI.DialogManagementService.Application.Queries.CreateQuery;
using PingAI.DialogManagementService.Application.Queries.DeleteQuery;
using PingAI.DialogManagementService.Application.Queries.GetQuery;
using PingAI.DialogManagementService.Application.Queries.ListQueries;
using PingAI.DialogManagementService.Application.Queries.UpdateDisplayOrders;
using PingAI.DialogManagementService.Application.Queries.UpdateQuery;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;
using PhrasePart = PingAI.DialogManagementService.Application.Queries.Shared.PhrasePart;
using Response = PingAI.DialogManagementService.Application.Queries.Shared.Response;

namespace PingAI.DialogManagementService.Api.Controllers
{
    [ApiVersion("1")]
    [ApiVersion("1.1")]
    [Authorize]
    public class QueriesController : ApiControllerBase
    {
        private readonly IMediator _mediator;

        public QueriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<QueryListItemDto[]>> ListQueries([FromQuery] Guid? projectId,
            [FromQuery] string? queryType,
            [FromQuery] bool? runtime)
        {
            if (!projectId.HasValue)
            {
                throw new BadRequestException($"{nameof(projectId)} must be provided");
            }

            if (!ValidateQueryType(queryType))
                throw new BadRequestException($"{nameof(queryType)} is invalid. Accepted types are " +
                                              $"{QueryTypes.Faq}, {QueryTypes.Handover}");

            var query = new ListQueriesQuery(projectId.Value, queryType, runtime == true);
            var queries = await _mediator.Send(query);
            return queries.Select(q => new QueryListItemDto(q)).ToArray();
        }

        private static readonly string[] ValidQueryTypes =
            typeof(QueryTypes).GetFields(BindingFlags.Public | BindingFlags.Static |
                                         BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
                .Select(fi => Convert.ToString(fi.GetValue(null)) ?? string.Empty)
                .ToArray();

        private static bool ValidateQueryType(string? queryType) =>
            queryType == null || ValidQueryTypes.Contains(queryType);

        [HttpGet("{queryId}")]
        public async Task<ActionResult<QueryDto>> GetQuery([FromRoute] Guid queryId)
        {
            var query = new GetQueryQuery(queryId);
            var result = await _mediator.Send(query);
            return new QueryDto(result);
        }

        [MapToApiVersion("1.1")]
        [HttpPost]
        public async Task<ActionResult<QueryDto>> CreateQuery([FromBody] CreateQueryDto request)
        {
            var projectId = Guid.Parse(request.ProjectId);
            var phraseParts = FlattenPhraseParts(request.Intent.PhraseParts).ToArray();
            var responses = request.Responses.Select(r =>
            {
                var formResolution = r.Form == null
                    ? null
                    : new FormResolution(
                        r.Form.Fields.Select(f => new FormField(f.DisplayName, f.Name)).ToArray());
                return new Response(r.RteText, formResolution, r.Order);
            }).ToArray();
            var createQueryCommand = new CreateQueryCommand(
                request.Name, projectId, phraseParts,
                new Expression[0], responses, request.Description ?? request.Name, request.Tags);
            var query = await _mediator.Send(createQueryCommand);
            return new QueryDto(query);
        }

        [HttpPost("displayOrders")]
        public async Task<IActionResult> UpdateDisplayOrders([FromBody] UpdateDisplayOrdersRequest request)
        {
            var command = new UpdateDisplayOrdersCommand(request.ProjectId, request.DisplayOrders);
            await _mediator.Send(command);
            return Ok();
        }

        [MapToApiVersion("1.1")]
        [HttpPut("{queryId}")]
        public async Task<ActionResult<QueryDto>> UpdateQuery(
            [FromRoute] Guid queryId,
            [FromBody] UpdateQueryDto request)
        {
            var phraseParts = FlattenPhraseParts(request.Intent.PhraseParts).ToArray();
            var responses = request.Responses.Select(r =>
            {
                var formResolution = r.Form == null
                    ? null
                    : new FormResolution(
                        r.Form.Fields.Select(f => new FormField(f.DisplayName, f.Name)).ToArray());
                return new Response(r.RteText, formResolution, r.Order);
            }).ToArray();
            var query = await _mediator.Send(new UpdateQueryCommand(
                queryId,
                request.Name,
                phraseParts,
                new Expression[0],
                responses, request.Description ?? request.Name,
                request.Tags, request.DisplayOrder));
            return new QueryDto(query);
        }

        private static IEnumerable<PhrasePart> FlattenPhraseParts(IEnumerable<CreatePhrasePartDto[]> partsGroups)
        {
            return from parts in partsGroups
                let phraseId = Guid.NewGuid()
                from part in parts
                select new PhrasePart(phraseId, Enum.Parse<PhrasePartType>(part.Type),
                    part.Position, part.Text, part.Value, part.EntityName);
        }

        [HttpDelete("{queryId}")]
        public async Task<IActionResult> DeleteQuery([FromRoute] Guid queryId)
        {
            await _mediator.Send(new DeleteQueryCommand(queryId));
            return Ok();
        }
    }
}
