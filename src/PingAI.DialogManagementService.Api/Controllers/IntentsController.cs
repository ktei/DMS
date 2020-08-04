using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PingAI.DialogManagementService.Api.Models.Intents;
using PingAI.DialogManagementService.Application.Intents.CreateIntent;
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

        [HttpPost]
        public async Task<ActionResult<CreateIntentResponse>> CreateIntent([FromBody] CreateIntentRequest request)
        {
            var intent = await _mediator.Send(new CreateIntentCommand(request.Name,
                Guid.Parse(request.ProjectId), Enum.Parse<IntentType>(request.Type, true)));
            return new CreateIntentResponse(intent);
        }
    }
}