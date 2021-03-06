using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Domain.Repositories
{
    public interface IIntentRepository
    {
        Task<IReadOnlyList<Intent>> ListByProjectId(Guid projectId);
        Task<Intent?> FindById(Guid intentId);
        Task<Intent> Add(Intent intent);
        void Remove(Intent intent);
    }
}
