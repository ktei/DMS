using System;
using System.Collections.Generic;

namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class ListQueriesResponse : List<QueryListItemDto>
    {
        public ListQueriesResponse(IEnumerable<QueryListItemDto> items)
        {
            AddRange(items ?? throw new ArgumentNullException(nameof(items)));
        }

        public ListQueriesResponse()
        {
            
        }
    }
}