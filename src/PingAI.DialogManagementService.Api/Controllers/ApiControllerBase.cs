using Microsoft.AspNetCore.Mvc;

namespace PingAI.DialogManagementService.Api.Controllers
{
    [ApiController]
    [Route("dms/api/v{version:apiVersion}/[controller]")]
    public class ApiControllerBase : ControllerBase
    {
        
    }
}