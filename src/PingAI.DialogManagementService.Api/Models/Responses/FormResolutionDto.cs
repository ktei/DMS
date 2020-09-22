using System.Linq;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Responses
{
    public class FormResolutionDto
    {
        public FieldDto[] Fields { get; set; }


        public FormResolutionDto(FormResolution form)
        {
            Fields = form.Fields.Select(f => new FieldDto(f.Name, f.DisplayName, f.DefaultValue)).ToArray();
        }

        public FormResolutionDto(FieldDto[] fields)
        {
            Fields = fields;
        }

        public FormResolutionDto()
        {
            
        }
    }

    public class FieldDto
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string? DefaultValue { get; set; }

        public FieldDto(string name, string displayName, string? defaultValue)
        {
            Name = name;
            DisplayName = displayName;
            DefaultValue = defaultValue;
        }

        public FieldDto()
        {
            
        }
    }
}