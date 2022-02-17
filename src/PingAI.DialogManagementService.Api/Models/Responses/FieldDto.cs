using System;

namespace PingAI.DialogManagementService.Api.Models.Responses
{
    public class FieldDto
    {
        public Guid EntityNameId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string? DefaultValue { get; set; }

        public FieldDto(
            Guid entityNameId,
            string name, string displayName, string? defaultValue)
        {
            EntityNameId = entityNameId;
            Name = name;
            DisplayName = displayName;
            DefaultValue = defaultValue;
        }

        public FieldDto()
        {
            
        }
    }
}