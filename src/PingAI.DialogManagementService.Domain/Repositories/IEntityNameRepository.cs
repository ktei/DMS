using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Domain.Repositories
{
    public interface IEntityNameRepository
    {
        Task<IReadOnlyList<EntityName>> ListByProjectId(Guid projectId);
        Task<EntityName?> FindByName(Guid projectId, string name);
        Task<EntityName> Add(EntityName entityName);
        void Remove(EntityName entityName);
    }
}