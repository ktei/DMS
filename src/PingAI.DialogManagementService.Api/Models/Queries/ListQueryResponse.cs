using System;
using System.Collections.Generic;

namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class ListQueryResponse : List<QueryListItemDto>
    {
        public ListQueryResponse(IEnumerable<QueryListItemDto> items)
        {
            AddRange(items ?? throw new ArgumentNullException(nameof(items)));
        }
    }
}