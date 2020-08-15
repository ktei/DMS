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
using PingAI.DialogManagementService.Application.Intents.UpdateIntent;
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
        public async Task<ActionResult<ListIntentsResponse>> ListIntents([FromQuery] Guid? projectId)
        {
            var query = new ListIntentsQuery(projectId);
            
            var results = await _mediator.Send(query);
            return new ListIntentsResponse(results.Select(x => new IntentListItemDto(x)));
        }

        [HttpGet("{intentId}")]
        public async Task<ActionResult<IntentDto>> GetIntent([FromRoute] Guid intentId)
        {
            var query = new GetIntentQuery(intentId);
            var intent = await _mediator.Send(query);
            return new IntentDto(intent);
        }

        [HttpPut("{intentId}")]
        public async Task<ActionResult<UpdateIntentResponse>> UpdateIntent([FromRoute] Guid intentId,
            [FromBody] UpdateIntentRequest request)
        {
            var phraseParts = new List<PhrasePart>();
            if (request.Phrases != null)
            {
                phraseParts.AddRange(from phrase in request.Phrases
                    let phraseId = Guid.NewGuid()
                    from part in phrase
                    select MapCreatePhrasePartDto(Guid.Empty, intentId, phraseId, part));
            }
            var intent = await _mediator.Send(new UpdateIntentCommand(intentId, request.Name, phraseParts));
            
            return new UpdateIntentResponse(intent);
        }

        [HttpPost]
        public async Task<ActionResult<CreateIntentResponse>> CreateIntent([FromBody] CreateIntentRequest request)
        {
            var intentId = Guid.NewGuid();
            var phraseParts = new List<PhrasePart>();
            if (request.Phrases != null)
            {
                phraseParts.AddRange(from phrase in request.Phrases
                    let phraseId = Guid.NewGuid()
                    from part in phrase
                    select MapCreatePhrasePartDto(Guid.Parse(request.ProjectId), intentId, phraseId, part));
            }

            var intent = await _mediator.Send(
                new CreateIntentCommand(intentId, request.Name, Guid.Parse(request.ProjectId),
                Enum.Parse<IntentType>(request.Type, true), phraseParts));
                
            return new CreateIntentResponse(intent);
        }

        private static PhrasePart MapCreatePhrasePartDto(Guid projectId, Guid intentId, Guid phraseId, CreatePhrasePartDto p)
        {
            var phrasePart = new PhrasePart(intentId, phraseId,
                p.Position, p.Text, p.Value, Enum.Parse<PhrasePartType>(p.Type),
                default(Guid?),
                p.EntityTypeId == null ? default(Guid?) : Guid.Parse(p.EntityTypeId));
            if (p.EntityName != null)
            {
                phrasePart.UpdateEntityName(new EntityName(p.EntityName, projectId, true));
            }

            return phrasePart;
        }
    }
}