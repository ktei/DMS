using System.Linq;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class QueryListItemDto
    {
        public string QueryId { get; set; }
        public string Name { get; set; }
        public string IntentPhrase { get; set; }
        public string ResponseText { get; set; }

        public QueryListItemDto(string queryId, string name, string intentPhrase, string responseText)
        {
            QueryId = queryId;
            Name = name;
            IntentPhrase = intentPhrase;
            ResponseText = responseText;
        }

        public QueryListItemDto(Query query) : this(query.Id.ToString(), query.Name,
            query.Intents.FirstOrDefault()?.GetPhrases().FirstOrDefault() ?? "N/A",
            query.Responses.FirstOrDefault()?.GetDisplayText())
        {
            
        }

        public QueryListItemDto()
        {
            
        }
    }
}