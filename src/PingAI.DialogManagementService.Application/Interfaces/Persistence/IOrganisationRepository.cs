using System;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Interfaces.Persistence
{
    public interface IOrganisationRepository
    {
        Task<Organisation?> GetOrganisationById(Guid id);
        Task<Organisation> AddOrganisation(Organisation organisation);
    }
}