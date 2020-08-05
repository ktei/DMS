using System;
using System.Threading.Tasks;

namespace PingAI.DialogManagementService.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<bool> UserCanReadProject(Guid projectId);
        Task<bool> UserCanWriteProject(Guid projectId);
    }
}