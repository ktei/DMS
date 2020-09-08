using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Interfaces.Persistence
{
    public interface IOrganisationRepository
    {
        Task<Organisation?> FindOrganisationByName(string name);
        Task<List<Organisation>> GetOrganisationsByIds(IEnumerable<Guid> ids);
        Task<Organisation> AddOrganisation(Organisation organisation);
    }
}