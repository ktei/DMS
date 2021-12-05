using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Domain.Repositories
{
    public interface IQueryRepository
    {
        Task<Query?> FindById(Guid queryId);
        Task<Guid?> FindProjectId(Guid queryId);
        Task<IReadOnlyList<Query>> ListByProjectId(Guid projectId, ResponseType? responseType);
        Task<Query> Add(Query query);
        Task<IReadOnlyList<Query>> ListByProjectIdWithoutJoins(Guid projectId);
        void Remove(Query query);
        Task<int> GetMaxDisplayOrder(Guid projectId);
    }
}
