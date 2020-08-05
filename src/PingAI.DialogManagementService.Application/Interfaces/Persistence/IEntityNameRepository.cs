using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Interfaces.Persistence
{
    public interface IEntityNameRepository
    {
        Task<List<EntityName>> GetEntityNamesByProjectId(Guid projectId);
        Task<EntityName> AddEntityName(EntityName entityName);
    }
}