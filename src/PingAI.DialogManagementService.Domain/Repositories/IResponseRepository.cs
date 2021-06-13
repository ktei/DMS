using System;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Domain.Repositories
{
    public interface IResponseRepository
    {
        Task<Response?> GetResponseById(Guid responseId);
        Task<Response> AddResponse(Response response);
    }
}