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

        public async Task<Organisation?> FindOrganisationByName(string name)
        {
            var organisation = await _context.Organisations
                .FirstOrDefaultAsync(o => o.Name == name);
            return organisation;
        }

        public async Task<List<Organisation>> GetOrganisationsByIds(IEnumerable<Guid> ids)
        {
            var _ = ids ?? throw new ArgumentNullException(nameof(ids));
            var organisationIds = ids as Guid[] ?? ids.ToArray();
            if (!organisationIds.Any()) return new List<Organisation>(0);
            return await _context.Organisations
                .Include(x => x.Projects)
                .Include(x => x.ProjectVersions)
                .Where(x => organisationIds.Contains(x.Id))
                .OrderBy(o => o.Name)
                .ToListAsync();
        }

        public async Task<Organisation> AddOrganisation(Organisation organisation)
        {
            var result = await _context.AddAsync(organisation);
            return result.Entity;
        }

        public Task<List<Organisation>> GetAllOrganisations()
        {
            return _context.Organisations
                .OrderBy(o => o.Name)
                .ToListAsync();
        }

        public Task<List<Organisation>> GetOrganisationsByUserId(Guid userId)
        {
            return _context.Organisations
                .Where(o => o.OrganisationUsers.Select(x => x.UserId).Contains(userId))
                .ToListAsync();
        }
    }
}