using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PingAI.DialogManagementService.Application.Integration.Slack;
using PingAI.DialogManagementService.Domain.ErrorHandling;

namespace PingAI.DialogManagementService.Api.Controllers.Integration
{
    [ApiVersion("1")]
    public class SlackController : IntegrationControllerBase
    {
        private readonly IMediator _mediator;

        public SlackController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpPost("approved")]
        public async Task<IActionResult> HandleSlackUserApprovedCallback(
            [FromQuery] Guid? projectId,
            [FromQuery] string? code,
            [FromQuery] string? redirectUri)
        {
            if (!projectId.HasValue)
            {
                throw new BadRequestException($"Missing {nameof(projectId)}");
            }

            if (string.IsNullOrEmpty(redirectUri))
            {
                throw new BadRequestException($"Missing {nameof(redirectUri)}");
            }

            if (string.IsNullOrEmpty(code))
            {
                throw new BadRequestException($"Missing {nameof(code)}");
            }
            var cookies = HttpContext.Request.Cookies;
            // var clientState = cookies["slack_auth_state"];
            // if (string.IsNullOrEmpty(clientState))
            //     throw new BadRequestException("Missing cookie slack_auth_state");

            await _mediator.Send(new ConnectSlackCommand(projectId.Value, code, "", redirectUri));
            return Ok();
        }
    }
}