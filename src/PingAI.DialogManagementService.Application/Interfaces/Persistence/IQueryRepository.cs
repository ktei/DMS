using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Interfaces.Persistence
{
    public interface IQueryRepository
    {
        Task<Query?> FindById(Guid queryId);
        Task<IReadOnlyList<Query>> ListByProjectId(Guid projectId, ResponseType? responseType);
        Task<Query> Add(Query query);
        Task<IReadOnlyList<Query>> ListByProjectIdWithoutJoins(Guid projectId);
        void Remove(Query query);
        Task<int> GetMaxDisplayOrder(Guid projectId);
    }
}
