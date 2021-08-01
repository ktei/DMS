namespace PingAI.DialogManagementService.Api.Models.Projects
{
    public class ProjectListItemOrganisation
    {
        public string OrganisationId { get; set; }
        public string Name { get; set; }

        public ProjectListItemOrganisation(string organisationId, string name)
        {
            OrganisationId = organisationId;
            Name = name;
        }

        public ProjectListItemOrganisation()
        {
                
        }
    }
}