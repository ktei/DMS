using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PingAI.DialogManagementService.Api.Controllers.Integration
{
    [Authorize]
    [Route("dms/api/integration/v{version:apiVersion}/[controller]")]
    public abstract class IntegrationControllerBase : ApiControllerBase
    {
        
    }
}