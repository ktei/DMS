using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PingAI.DialogManagementService.Api.Models.Webhooks;
using PingAI.DialogManagementService.Application.Webhooks.CreateWebhook;
using PingAI.DialogManagementService.Application.Webhooks.DeleteWebhook;
using PingAI.DialogManagementService.Application.Webhooks.ListWebhooks;
using PingAI.DialogManagementService.Application.Webhooks.UpdateWebhook;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Repositories;
using IAuthorizationService = PingAI.DialogManagementService.Application.Interfaces.Services.Security.IAuthorizationService;

namespace PingAI.DialogManagementService.Api.Controllers
{
    [ApiVersion("1")]
    [Authorize]
    public class WebhooksController : ApiControllerBase
    {
        private readonly IMediator _mediator;

        public WebhooksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<WebhookListItem>>> ListWebhooks([FromQuery] Guid? projectId)
        {
            if (!projectId.HasValue)
                throw new BadRequestException($"{nameof(projectId)} must be provided.");
            var query = new ListWebhooksQuery(projectId.Value);
            var results = await _mediator.Send(query);
            return results.Select(r => new WebhookListItem(r.Id, 
                    r.Resolution!.Webhook!.EntityName))
                .ToImmutableList();
        }

        [HttpGet("{responseId}")]
        public async Task<ActionResult<WebhookDetails>> GetWebhook([FromRoute] Guid responseId,
            [FromServices] IResponseRepository responseRepository,
            [FromServices] IAuthorizationService authorizationService)
        {
            var response = await responseRepository.FindById(responseId);
            if (response == null)
                throw new NotFoundException("Webhook was not found.");
            var canReadProject = await authorizationService.UserCanReadProject(response.ProjectId);
            if (!canReadProject)
                throw new UnauthorizedException(ErrorDescriptions.ProjectReadDenied);
            return new WebhookDetails(response);
        }
        
        [HttpPost]
        public async Task<ActionResult<WebhookDetails>> CreateWebhook([FromBody] CreateWebhookRequest request)
        {
            var command = new CreateWebhookCommand(request.ProjectId,
                request.Name, request.Method, request.Url,
                request.Headers);
            var result = await _mediator.Send(command);
            return new WebhookDetails(result.Response);
        }

        [HttpPut("{responseId}")]
        public async Task<ActionResult<WebhookDetails>> UpdateWebhook([FromRoute] Guid responseId,
            [FromBody] UpdateWebhookRequest request)
        {
            var command = new UpdateWebhookCommand(responseId,
                request.Method, request.Url, request.Headers);
            var result = await _mediator.Send(command);
            return new WebhookDetails(result);
        }
        
        [HttpDelete("{responseId}")]
        public async Task<IActionResult> DeleteWebhook([FromRoute] Guid responseId)
        {
            var command = new DeleteWebhookCommand(responseId);
            await _mediator.Send(command);
            return Ok();
        }
    }
}
