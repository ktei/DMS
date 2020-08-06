using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Intents
{
    public class IntentListItemDto
    {
        public string IntentId { get; set; }
        public string Name { get; set; }

        public IntentListItemDto(string intentId, string name)
        {
            IntentId = intentId;
            Name = name;
        }

        public IntentListItemDto(Intent intent) : this(intent.Id.ToString(), intent.Name)
        {
            
        }

        public IntentListItemDto()
        {
            
        }
    }
}
