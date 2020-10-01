using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class QueryListItemIntent
    {
        public string IntentId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        public QueryListItemIntent(string intentId, string name, string type)
        {
            IntentId = intentId;
            Name = name;
            Type = type;
        }

        public QueryListItemIntent(Intent intent) : this(intent.Id.ToString(), intent.Name, intent.Type.ToString())
        {

        }


        public QueryListItemIntent()
        {
            
        }
    }
}