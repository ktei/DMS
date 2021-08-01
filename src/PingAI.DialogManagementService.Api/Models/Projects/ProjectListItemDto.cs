using System;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Projects
{
    public class ProjectListItemDto
    {
        public string ProjectId { get; set; }
        public string Name { get; set; }
        public ProjectListItemOrganisation Organisation { get; set; }
        public string[] Domains { get; set; }
        public string CreatedAt { get; set; }

        public ProjectListItemDto(Project project)
        {
            ProjectId = project.Id.ToString();
            Name = project.Name;
            Organisation = new ProjectListItemOrganisation(project.Organisation!.Id.ToString(), project.Organisation.Name);
            Domains = project.Domains ?? new string[0];
            CreatedAt = project.CreatedAt.ToString("o");
        }

        public ProjectListItemDto()
        {
            
        }
    }
}