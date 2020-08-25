using System;

namespace PingAI.DialogManagementService.Api.Models.Projects
{
    public class ProjectListItemDto
    {
        public string ProjectId { get; set; }
        public string Name { get; set; }

        public ProjectListItemDto(string projectId, string name)
        {
            ProjectId = projectId;
            Name = name;
        }

        public ProjectListItemDto()
        {
            
        }
    }
}