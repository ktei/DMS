using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Interfaces.Persistence
{
    public interface IResponseRepository
    {
        Task<Response> AddResponse(Response response);
    }
}