using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Interfaces.Persistence
{
    public interface IQueryRepository
    {
        Task<Query?> GetQueryByIdWithoutJoins(Guid queryId);
        Task<Query?> GetQueryById(Guid queryId);
        Task<Query> AddQuery(Query query);
        Task<List<Query>> GetQueriesByProjectId(Guid projectId);
        Task<List<Query>> GetQueriesByProjectId(Guid projectId, ResponseType? responseType);
        void RemoveQuery(Query query);
        Task<int> GetMaxDisplayOrder(Guid projectId);
    }
}