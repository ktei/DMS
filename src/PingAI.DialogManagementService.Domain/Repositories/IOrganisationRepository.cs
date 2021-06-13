using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Domain.Repositories
{
    public interface IOrganisationRepository
    {
        Task<Organisation?> FindByName(string name);
        Task<Organisation> Add(Organisation organisation);

        // TODO: if we really take off, we need to paginate this,
        // but we're far from that right now
        Task<IReadOnlyList<Organisation>> ListAll();
        Task<IReadOnlyList<Organisation>> ListByUserId(Guid userId);
    }
}