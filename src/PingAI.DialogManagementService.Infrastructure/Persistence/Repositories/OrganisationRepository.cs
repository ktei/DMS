using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Repositories
{
    public class OrganisationRepository : IOrganisationRepository
    {
        private readonly DialogManagementContext _context;

        public OrganisationRepository(DialogManagementContext context)
        {
            _context = context;
        }

        public Task<Organisation?> GetOrganisationById(Guid id) =>
            _context.Organisations.FirstOrDefaultAsync(x => x.Id == id);

        public async Task<Organisation> AddOrganisation(Organisation organisation)
        {
            var result = await _context.AddAsync(organisation);
            return result.Entity;
        }
    }
}