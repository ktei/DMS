using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Queries
{
    public class QueryListItemIntent
    {
        public string Name { get; set; }
        public string Type { get; set; }

        public QueryListItemIntent(string name, string type)
        {
            Name = name;
            Type = type;
        }

        public QueryListItemIntent(Intent intent) : this(intent.Name, intent.Type.ToString())
        {
            
        }
        

        public QueryListItemIntent()
        {
            
        }
    }
}