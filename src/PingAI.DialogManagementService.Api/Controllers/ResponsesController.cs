using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PingAI.DialogManagementService.Api.Models.Responses;
using PingAI.DialogManagementService.Application.Responses.CreateResponse;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Controllers
{
    [ApiVersion("1")]
    [Authorize]
    public class ResponsesController : ApiControllerBase
    {
        private readonly IMediator _mediator;

        public ResponsesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<ResponseDto>> CreateResponse([FromBody] CreateResponseRequest request)
        {
            var response = await _mediator.Send(new CreateResponseCommand(Guid.Parse(request.ProjectId),
                Enum.Parse<ResponseType>(request.Type, true), request.RteText, request.Order));
            return new ResponseDto(response);
        }
    }
}