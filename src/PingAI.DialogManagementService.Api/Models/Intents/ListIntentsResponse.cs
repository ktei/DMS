using System;
using System.Collections.Generic;

namespace PingAI.DialogManagementService.Api.Models.Intents
{
    public class ListIntentsResponse : List<IntentListItemDto>
    {
        public ListIntentsResponse(IEnumerable<IntentListItemDto> items)
        {
            AddRange(items ?? throw new ArgumentNullException(nameof(items)));
        }
    }
}