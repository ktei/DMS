using System;
using System.Linq;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class QueryListItemDto
    {
        public string QueryId { get; set; }
        
        public QueryListItemIntent Intent { get; set; }
        public QueryListItemResponse Response { get; set; }
        
        [Obsolete("Deprecated. Front-end should use Response.Text")]
        public string ResponseText { get; set; }

        public QueryListItemDto(string queryId, QueryListItemIntent intent,
            QueryListItemResponse response,
            string responseText)
        {
            QueryId = queryId;
            Intent = intent;
            Response = response;
            ResponseText = responseText;
        }

        public QueryListItemDto(Query query)
        {
            QueryId = query.Id.ToString();
            Intent = new QueryListItemIntent(query.Intents.First());
            Response = new QueryListItemResponse(query.Responses.First());
            ResponseText = Response.Text ?? string.Empty;
        }

        public QueryListItemDto()
        {
            
        }
    }
}
