using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Interfaces.Persistence
{
    public interface IIntentRepository
    {
        Task<List<Intent>> GetIntentsByProjectId(Guid projectId);
        Task<Intent> AddIntent(Intent intent);
    }
}