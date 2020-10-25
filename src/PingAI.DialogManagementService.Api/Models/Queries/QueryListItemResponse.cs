using System.Linq;
using System.Text;
using PingAI.DialogManagementService.Api.Models.Responses;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class QueryListItemResponse
    {
        public string ResponseId { get; set; }
        public string? Text { get; set; }
        public string Type { get; set; }
        public ResolutionDto Resolution { get; set; }

        public QueryListItemResponse(string responseId, string type, string? text, ResolutionDto resolution)
        {
            ResponseId = responseId;
            Type = type;
            Text = text;
            Resolution = resolution;
        }

        public QueryListItemResponse(Response response)
        {
            ResponseId = response.Id.ToString();
            Type = response.Type.ToString();
            Text = GetResponseText(response);
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
        }

        public QueryListItemResponse()
        {
            
        }

        private static string? GetResponseText(Response response)
        {
            if (response.Resolution?.Parts == null)
                return null;

            var textBuilder = new StringBuilder();
            foreach (var resolutionPart in response.Resolution.Parts)
            {
                textBuilder.Append(resolutionPart.Text);
            }

            return textBuilder.ToString();
        }
    }
}