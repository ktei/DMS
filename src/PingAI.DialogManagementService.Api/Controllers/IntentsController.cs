using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PingAI.DialogManagementService.Api.Models.Intents;
using PingAI.DialogManagementService.Application.Intents.CreateIntent;
using PingAI.DialogManagementService.Application.Intents.GetIntent;
using PingAI.DialogManagementService.Application.Intents.ListIntents;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Controllers
{
    [ApiVersion("1")]
    [Authorize]
    public class IntentsController : ApiControllerBase
    {
        private readonly IMediator _mediator;

        public IntentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<IntentListItemDto>>> ListIntents([FromQuery] Guid? projectId)
        {
            var query = new ListIntentsQuery(projectId);
            
            var results = await _mediator.Send(query);
            return results.Select(x => new IntentListItemDto(x)).ToList();
        }

        [HttpGet("{intentId}")]
        public async Task<ActionResult<IntentDto>> GetIntent([FromRoute] Guid intentId)
        {
            var query = new GetIntentQuery(intentId);
            var intent = await _mediator.Send(query);
            return new IntentDto(intent);
        }

        [HttpPost]
        public async Task<ActionResult<CreateIntentResponse>> CreateIntent([FromBody] CreateIntentRequest request)
        {
            var intentId = Guid.NewGuid();
            var phraseParts = request.PhraseParts?.Select(p => 
                MapCreatePhrasePartDto(Guid.Parse(request.ProjectId), intentId, p));
            var intent = await _mediator.Send(
                new CreateIntentCommand(intentId, request.Name, Guid.Parse(request.ProjectId),
                Enum.Parse<IntentType>(request.Type, true), phraseParts));
                
            return new CreateIntentResponse(intent);
        }

        private static PhrasePart MapCreatePhrasePartDto(Guid projectId, Guid intentId, CreatePhrasePartDto p)
        {
            var phrasePart = new PhrasePart(Guid.NewGuid(), intentId, Guid.Parse(p.PhraseId),
                p.Position, p.Text, p.Value, Enum.Parse<PhrasePartType>(p.Type),
                null,
                p.EntityTypeId == null ? default(Guid?) : Guid.Parse(p.EntityTypeId));
            if (p.EntityName != null)
            {
                phrasePart.UpdateEntityName(new EntityName(Guid.NewGuid(), p.EntityName, projectId, true));
            }

            return phrasePart;
        }
    }
}