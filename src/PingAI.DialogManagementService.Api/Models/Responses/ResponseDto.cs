using System.Linq;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Responses
{
    public class ResponseDto
    {
        public string ResponseId { get; set; }
        public string ProjectId { get; set; }
        public string Type { get; set; }
        public ResolutionDto Resolution { get; set; }
        public int Order { get; set; }
        public string? SpeechContexts { get; set; }
        
        public ResponseDto(Response response)
        {
            ResponseId = response.Id.ToString();
            ProjectId = response.ProjectId.ToString();
            Type = response.Type.ToString();
            Order = response.Order;
            if (response.Resolution?.Type == ResolutionType.PARTS)
            {
                var parts = response.Resolution.Parts?
                    .Select(p => new ResolutionPartDto(p)).ToArray();
                Resolution = new ResolutionDto(parts, null, ResolutionType.PARTS.ToString());
            }
            else if (response.Resolution?.Type == ResolutionType.FORM)
            {
                var form = response.Resolution.Form == null ? default : new FormResolutionDto(response.Resolution.Form);
                Resolution = new ResolutionDto(null, form, ResolutionType.FORM.ToString());
            }
            else
            {
                Resolution = new ResolutionDto(null, null, ResolutionType.EMPTY.ToString());
            }

            SpeechContexts = response.SpeechContexts;
        }

        public ResponseDto()
        {
            
        }
    }
}