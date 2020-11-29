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
using PingAI.DialogManagementService.Application.Queries.Insert;
using PingAI.DialogManagementService.Application.Queries.ListQueries;
using PingAI.DialogManagementService.Application.Queries.UpdateQuery;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;

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
        public async Task<ActionResult<List<QueryListItemDto>>> ListQueries([FromQuery] Guid? projectId,
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
            return new ListQueriesResponse(queries.Select(q => new QueryListItemDto(q)));
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

        [MapToApiVersion("1")]
        [HttpPost]
        public async Task<ActionResult<QueryDto>> CreateQuery([FromBody] CreateQueryRequest request)
        {
            var projectId = Guid.Parse(request.ProjectId);
            var phraseParts = FlattenPhraseParts(projectId, Guid.Empty, request.Intent.PhraseParts);
            var intent = new Intent(request.Intent.Name, projectId, IntentType.STANDARD, phraseParts);
            var response = new Response(projectId, ResponseType.RTE, request.Response.Order);
            var query = await _mediator.Send(new CreateQueryCommand(
                request.Name, Guid.Parse(request.ProjectId),
                new Expression[0], request.Description ?? request.Name, request.Tags,
                request.DisplayOrder, null, intent, null, new[] {response}, request.Response.RteText));
            return new QueryDto(query);
        }

        [MapToApiVersion("1.1")]
        [HttpPost]
        public async Task<ActionResult<QueryDto>> CreateQueryV1_1([FromBody] CreateQueryRequestV1_1 request)
        {
            var projectId = Guid.Parse(request.ProjectId);
            var phraseParts = FlattenPhraseParts(projectId, Guid.Empty, request.Intent.PhraseParts);
            var intent = new Intent(request.Intent.Name, projectId, IntentType.STANDARD, phraseParts);
            var responses = request.Responses.Select(r => r.Form switch
            {
                { } form when true => new Response(new Resolution(new FormResolution(
                        form.Fields.Select(f => new FormResolution.Field(f.DisplayName, f.Name)).ToArray())),
                    projectId, ResponseType.FORM, r.Order),
                _ => new Response(projectId, Enum.Parse<ResponseType>(r.Type), r.Order)
            }).ToArray();
            var query = await _mediator.Send(new CreateQueryCommand(
                request.Name, Guid.Parse(request.ProjectId),
                new Expression[0], request.Description ?? request.Name, request.Tags,
                request.DisplayOrder, null, intent, null, responses,
                request.Responses.FirstOrDefault(r => r.Type == ResponseType.RTE.ToString())?.RteText));
            return new QueryDto(query);
        }

        [MapToApiVersion("1")]
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
                request.DisplayOrder, null, intent, null, new[] {response}, request.Response.RteText));
            return new UpdateQueryResponse(query);
        }

        [HttpPost("insert")]
        public async Task<IActionResult> Insert([FromBody] InsertRequest request)
        {
            var command = new InsertCommand(
                Guid.Parse(request.QueryId), request.DisplayOrder);
            await _mediator.Send(command);
            return Ok();
        }

        [MapToApiVersion("1.1")]
        [HttpPut("{queryId}")]
        public async Task<ActionResult<UpdateQueryResponse>> UpdateQueryV1_1(
            [FromRoute] Guid queryId,
            [FromBody] UpdateQueryRequestV1_1 request)
        {
            var phraseParts = FlattenPhraseParts(Guid.Empty, Guid.Empty, request.Intent.PhraseParts);
            var intent = new Intent(request.Intent.Name, Guid.Empty, IntentType.STANDARD, phraseParts);
            var responses = request.Responses.Select(r =>
            { 
                if (r.Form != null)
                {
                    return new Response(new Resolution(new FormResolution(
                        r.Form.Fields.Select(f => new FormResolution.Field(f.DisplayName, f.Name)).ToArray())),
                        Guid.Empty, ResponseType.FORM, r.Order);
                }
                return new Response(Guid.Empty, Enum.Parse<ResponseType>(r.Type), r.Order);
            }).ToArray();
            var query = await _mediator.Send(new UpdateQueryCommand(
                queryId,
                request.Name,
                new Expression[0], request.Description ?? request.Name, request.Tags,
                request.DisplayOrder, null, intent, null, responses,
                request.Responses.FirstOrDefault(r => r.Type == ResponseType.RTE.ToString())?.RteText));
            return new UpdateQueryResponse(query);
        }

        private static IEnumerable<PhrasePart> FlattenPhraseParts(Guid projectId, Guid intentId,
            CreatePhrasePartDto[][] partsGroups)
        {
            var displayOrder = 1;
            foreach (var parts in partsGroups)
            {
                var phraseId = Guid.NewGuid();
                foreach (var part in parts)
                {
                    yield return new PhrasePart(intentId, phraseId, part.Position, part.Text,
                        part.Value, Enum.Parse<PhrasePartType>(part.Type),
                        string.IsNullOrEmpty(part.EntityName) ? null : new EntityName(part.EntityName, projectId, true),
                        string.IsNullOrEmpty(part.EntityTypeId) ? default(Guid?) : Guid.Parse(part.EntityTypeId), displayOrder);
                }

                displayOrder++;
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