using System.Text;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class QueryListItemResponse
    {
        public string ResponseId { get; set; }
        public string? Text { get; set; }
        public string Type { get; set; }

        public QueryListItemResponse(string responseId, string type, string? text)
        {
            ResponseId = responseId;
            Type = type;
            Text = text;
        }

        public QueryListItemResponse(Response response)
        {
            ResponseId = response.Id.ToString();
            Type = response.Type.ToString();
            Text = GetResponseText(response);
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