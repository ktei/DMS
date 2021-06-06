using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Interfaces.Persistence
{
    public interface IEntityTypeRepository
    {
        Task<IReadOnlyList<EntityType>> ListByProjectId(Guid projectId);
    }
}