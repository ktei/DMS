using System;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Intents
{
    public class IntentDto
    {
        public string IntentId { get; set; }
        public string Name { get; set; }
        public string ProjectId { get; set; }
        public string Type { get; set; }

        public IntentDto(string intentId, string name, string projectId, string type)
        {
            IntentId = intentId;
            Name = name;
            ProjectId = projectId;
            Type = type;
        }

        public IntentDto(Intent intent) : this(intent.Id.ToString(),
            intent.Name, intent.ProjectId.ToString(), intent.Type.ToString())
        {
            
        }

        public IntentDto()
        {
            
        }
    }
}