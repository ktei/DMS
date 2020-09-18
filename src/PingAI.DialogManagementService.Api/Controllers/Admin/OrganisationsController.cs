using System;
using System.Collections.Generic;
using System.Linq;
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

        [HttpGet]
        public async Task<List<OrganisationListItemDto>> ListOrganisations([FromQuery] Guid? userId)
        {
            var organisations = await _mediator.Send(new ListOrganisationsQuery(userId));
            return organisations.Select(o => new OrganisationListItemDto(o)).ToList();
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