using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Intents
{
    public class UpdateIntentResponse : IntentDto
    {
        public UpdateIntentResponse(Intent intent) : base(intent)
        {
            
        }
        
        public UpdateIntentResponse()
        {
            
        }
    }
}