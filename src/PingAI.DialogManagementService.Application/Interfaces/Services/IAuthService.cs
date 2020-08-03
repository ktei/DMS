using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<bool> UserCanWriteProject(Project project);
    }
}