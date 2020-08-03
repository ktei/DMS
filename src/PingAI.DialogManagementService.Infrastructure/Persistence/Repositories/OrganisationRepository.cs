using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<List<Organisation>> GetOrganisationsByIds(IEnumerable<Guid> ids)
        {
            var _ = ids ?? throw new ArgumentNullException(nameof(ids));
            var organisationIds = ids as Guid[] ?? ids.ToArray();
            if (!organisationIds.Any()) return new List<Organisation>(0);
            return await _context.Organisations
                .Include(x => x.Projects)
                .Where(x => organisationIds.Contains(x.Id))
                .ToListAsync();
        }


        public async Task<Organisation> AddOrganisation(Organisation organisation)
        {
            var result = await _context.AddAsync(organisation);
            return result.Entity;
        }
    }
}