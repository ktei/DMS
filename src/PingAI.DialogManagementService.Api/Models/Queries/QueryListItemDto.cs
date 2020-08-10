using System;

namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class QueryListItemDto
    {
        public string QueryId { get; set; }
        public string Name { get; set; }
        public string IntentPhrase { get; set; }
        public string Response { get; set; }
    }
}