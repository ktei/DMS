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
using PingAI.DialogManagementService.Application.Queries.UpdateDisplayOrders;
using PingAI.DialogManagementService.Application.Queries.UpdateQuery;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;
using PhrasePart = PingAI.DialogManagementService.Application.Queries.CreateQuery.PhrasePart;
using Response = PingAI.DialogManagementService.Application.Queries.CreateQuery.Response;

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

        [MapToApiVersion("1.1")]
        [HttpPost]
        public async Task<ActionResult<QueryDto>> CreateQuery([FromBody] CreateQueryRequestV1_1 request)
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
                request.Name, Guid.Parse(request.ProjectId), phraseParts,
                new Expression[0], responses, request.Description ?? request.Name, request.Tags);
            var query = await _mediator.Send(createQueryCommand);
            return new QueryDto(query);
        }

        [HttpPost("insert")]
        public async Task<IActionResult> Insert([FromBody] InsertRequest request)
        {
            var command = new InsertCommand(
                Guid.Parse(request.QueryId), request.DisplayOrder);
            await _mediator.Send(command);
            return Ok();
        }

        [HttpPost("displayOrders")]
        public async Task<IActionResult> UpdateDisplayOrders([FromBody] UpdateDisplayOrdersRequest request)
        {
            var command = new UpdateDisplayOrdersCommand(request.ProjectId, request.DisplayOrders);
            await _mediator.Send(command);
            return Ok();
        }

        // [MapToApiVersion("1.1")]
        // [HttpPut("{queryId}")]
        // public async Task<ActionResult<UpdateQueryResponse>> UpdateQueryV1_1(
        //     [FromRoute] Guid queryId,
        //     [FromBody] UpdateQueryRequestV1_1 request)
        // {
        //     var phraseParts = FlattenPhraseParts(request.Intent.PhraseParts);
        //     var responses = request.Responses.Select(r =>
        //     {
        //         var fields = r.Form?.Fields.Select(f => new FormResolution.Field(f.DisplayName, f.Name));
        //         return new Response(r.RteText, fields == null ? null : new FormResolution(fields), r.Order);
        //     }).ToArray();
        //     var query = await _mediator.Send(new UpdateQueryCommand(
        //         queryId,
        //         request.Name,
        //         new Expression[0], request.Description ?? request.Name, request.Tags,
        //         request.DisplayOrder, null, intent, null, responses,
        //         request.Responses.Select(r => r.RteText).ToArray()));
        //     return new UpdateQueryResponse(query);
        // }

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