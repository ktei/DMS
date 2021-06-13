using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Interfaces.Services.Security
{
    public interface IIdentityContext
    {
        Task<User> GetUser();
    }
}
