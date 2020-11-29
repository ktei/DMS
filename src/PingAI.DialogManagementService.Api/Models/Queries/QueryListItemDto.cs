using System;
using System.Linq;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class QueryListItemDto
    {
        public string QueryId { get; set; }
        public int DisplayOrder { get; set; }
        
        public QueryListItemIntent Intent { get; set; }
        public QueryListItemResponse[] Responses { get; set; }
        
        [Obsolete("Deprecated. Front-end should use Response.Text")]
        public string ResponseText { get; set; }

        public QueryListItemDto(string queryId,
            int displayOrder,
            QueryListItemIntent intent,
            QueryListItemResponse[] responses,
            string responseText)
        {
            QueryId = queryId;
            DisplayOrder = displayOrder;
            Intent = intent;
            Responses = responses;
            ResponseText = responseText;
        }

        public QueryListItemDto(Query query)
        {
            QueryId = query.Id.ToString();
            DisplayOrder = query.DisplayOrder;
            Intent = new QueryListItemIntent(query.Intents.First());
            Responses = query.Responses.Select(r => new QueryListItemResponse(r)).ToArray();
            ResponseText = Responses.FirstOrDefault()?.Text ?? string.Empty;
        }

        public QueryListItemDto()
        {
            
        }
    }
}
