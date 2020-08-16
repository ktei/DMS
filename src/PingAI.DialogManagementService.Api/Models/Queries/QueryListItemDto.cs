using System.Linq;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class QueryListItemDto
    {
        public string QueryId { get; set; }
        
        public QueryListItemIntent Intent { get; set; }
        public string ResponseText { get; set; }

        public QueryListItemDto(string queryId, QueryListItemIntent intent, string responseText)
        {
            QueryId = queryId;
            Intent = intent;
            ResponseText = responseText;
        }

        public QueryListItemDto(Query query) : this(query.Id.ToString(),
            new QueryListItemIntent(query.Intents.First()),
            query.Responses.FirstOrDefault()?.GetDisplayText())
        {
            
        }

        public QueryListItemDto()
        {
            
        }
    }
}