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
        public ResolutionPartDto[]? Parts { get; set; }
        public FormResolutionDto? Form { get; set; }

        public QueryListItemResponse(string responseId, string type, string? text,
            ResolutionPartDto[]? parts, FormResolutionDto? form)
        {
            ResponseId = responseId;
            Type = type;
            Text = text;
            Parts = parts;
            Form = form;
        }

        public QueryListItemResponse(Response response)
        {
            ResponseId = response.Id.ToString();
            Type = response.Type.ToString();
            Text = GetResponseText(response);
            if (response.Resolution?.Type == ResolutionType.PARTS)
            {
                Parts = response.Resolution.Parts?.Select(p => new ResolutionPartDto(p)).ToArray();
            }
            else if (response.Resolution?.Type == ResolutionType.FORM)
            {
                Form = response.Resolution.Form == null ? default : new FormResolutionDto(response.Resolution.Form);
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