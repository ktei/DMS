using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Webhooks.CreateWebhook
{
    public class CreateWebhookResult
    {
        public EntityName EntityName { get; }
        public Response Response { get; }

        public CreateWebhookResult(EntityName entityName, Response response)
        {
            EntityName = entityName;
            Response = response;
        }
    }
}