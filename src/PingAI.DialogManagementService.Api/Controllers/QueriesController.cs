using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PingAI.DialogManagementService.Api.Models.Queries;
using PingAI.DialogManagementService.Application.Queries.CreateQuery;

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

        [HttpPost]
        public async Task<ActionResult<CreateQueryResponse>> CreateQuery([FromBody] CreateQueryRequest request)
        {
            throw new NotImplementedException();
        }
    }
}