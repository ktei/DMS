using System;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Organisations
{
    public class OrganisationDto
    {
        public string OrganisationId { get; set; }
        public string Name { get; set; }

        public OrganisationDto(Organisation organisation)
        {
            _ = organisation ?? throw new ArgumentNullException(nameof(organisation));
            OrganisationId = organisation.Id.ToString();
            Name = organisation.Name;
        }

        public OrganisationDto()
        {
            
        }
    }
}