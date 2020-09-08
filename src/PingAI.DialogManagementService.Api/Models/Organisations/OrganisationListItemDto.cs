using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Organisations
{
    public class OrganisationListItemDto
    {
        public string OrganisationId { get; set; }
        public string Name { get; set; }

        public OrganisationListItemDto(Organisation organisation)
        {
            OrganisationId = organisation.Id.ToString();
            Name = organisation.Name;
        }

        public OrganisationListItemDto()
        {
            
        }
    }
}