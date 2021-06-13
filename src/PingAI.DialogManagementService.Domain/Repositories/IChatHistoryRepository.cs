using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Domain.Repositories
{
    public interface IChatHistoryRepository
    {
        Task<List<ChatHistory>> GetChatHistories(Guid designTimeProjectId, 
            DateTime timeRangeStartUtc, DateTime? timeRangeEndUtc);
    }
}
