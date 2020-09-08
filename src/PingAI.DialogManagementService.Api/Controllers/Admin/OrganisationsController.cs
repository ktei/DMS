using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PingAI.DialogManagementService.Api.Models.Organisations;
using PingAI.DialogManagementService.Application.Admin.Organisations;

namespace PingAI.DialogManagementService.Api.Controllers.Admin
{
    [ApiVersion("1")]
    public class OrganisationsController : AdminControllerBase
    {
        private readonly IMediator _mediator;

        public OrganisationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<OrganisationDto> CreateOrganisation([FromBody] CreateOrganisationRequest request)
        {
            var organisation = await _mediator.Send(
                new CreateOrganisationCommand(request.Name, request.AdminUserId));
            return new OrganisationDto(organisation);
        }
    }
}