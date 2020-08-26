using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PingAI.DialogManagementService.Api.Authorization;

namespace PingAI.DialogManagementService.Api.Controllers.Admin
{
    [Authorize(Policy = Policies.AdminOnly)]
    [Route("dms/api/admin/v{version:apiVersion}/[controller]")]
    public abstract class AdminControllerBase : ApiControllerBase
    {
        
    }
}