using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Interfaces.Persistence
{
    public interface IEntityTypeRepository
    {
        Task<List<EntityType>> GetEntityTypesByProjectId(Guid projectId);
        Task<EntityType> AddEntityType(EntityType entityType);
    }
}