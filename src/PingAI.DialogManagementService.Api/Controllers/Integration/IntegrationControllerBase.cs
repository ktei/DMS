using Microsoft.AspNetCore.Mvc;

namespace PingAI.DialogManagementService.Api.Controllers.Integration
{
    [Route("dms/api/integration/v{version:apiVersion}/[controller]")]
    public abstract class IntegrationControllerBase : ApiControllerBase
    {
        
    }
}