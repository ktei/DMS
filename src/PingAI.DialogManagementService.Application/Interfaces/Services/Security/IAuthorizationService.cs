using System;
using System.Threading.Tasks;

namespace PingAI.DialogManagementService.Application.Interfaces.Services.Security
{
    public interface IAuthorizationService
    {
        Task<bool> UserCanReadProject(Guid projectId);
        Task<bool> UserCanWriteProject(Guid projectId);
        Task<bool> HasAdminPrivilege();
    }
}
