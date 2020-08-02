using System.Threading.Tasks;

namespace PingAI.DialogManagementService.Application.Interfaces.Persistence
{
    public interface IUnitOfWork
    {
        Task SaveChanges();
    }
}