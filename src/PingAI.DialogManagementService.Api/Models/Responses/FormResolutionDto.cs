using System.Linq;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Responses
{
    public class FormResolutionDto
    {
        public FieldDto[] Fields { get; set; }

        public FormResolutionDto(FormResolution form)
        {
            Fields = form.Fields.Select(f => new FieldDto(
                f.EntityNameId,
                f.Name, f.DisplayName, f.DefaultValue)).ToArray();
        }

        public FormResolutionDto(FieldDto[] fields)
        {
            Fields = fields;
        }

        public FormResolutionDto()
        {
            
        }
    }
}