using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Interfaces.Persistence
{
    public interface IQueryRepository
    {
        Task<Query> AddQuery(Query query);
    }
}