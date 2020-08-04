using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Intents
{
    public class CreateIntentResponse : IntentDto
    {
        public CreateIntentResponse(Intent intent) : base(intent)
        {
            
        }
        
        public CreateIntentResponse()
        {
            
        }
    }
}