using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Domain.Repositories
{
    public interface IEntityTypeRepository
    {
        Task<IReadOnlyList<EntityType>> ListByProjectId(Guid projectId);
    }
}