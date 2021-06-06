using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Interfaces.Persistence
{
    public interface IEntityNameRepository
    {
        Task<IReadOnlyList<EntityName>> ListByProjectId(Guid projectId);
        Task<EntityName> Add(EntityName entityName);
    }
}