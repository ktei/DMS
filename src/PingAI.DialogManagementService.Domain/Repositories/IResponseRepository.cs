using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Domain.Repositories
{
    public interface IResponseRepository
    {
        Task<Response?> FindById(Guid responseId);
        Task<IReadOnlyList<Response>> ListByProjectId(Guid projectId, ResponseType responseType);
        Task<Response> Add(Response response);
        void Remove(Response response);
        void RemoveRange(IEnumerable<Response> responses);
    }
}