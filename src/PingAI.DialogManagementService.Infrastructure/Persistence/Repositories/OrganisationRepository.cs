using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

        public async Task<Organisation?> FindById(Guid id)
        {
            var organisation = await _context.Organisations
                .Include(x => x.Users)
                .Include(x => x.Projects)
                .Include(x => x.ProjectVersions)
                .SingleOrDefaultAsync(o => o.Id == id);
            return organisation; 
        }

        public async Task<Organisation?> FindByName(string name)
        {
            var organisation = await _context.Organisations
                .FirstOrDefaultAsync(o => o.Name == name);
            return organisation;
        }
        
        public async Task<Organisation> Add(Organisation organisation)
        {
            var result = await _context.AddAsync(organisation);
            return result.Entity;
        }

        public async Task<IReadOnlyList<Organisation>> ListAll()
        {
            var results = await _context.Organisations
                .OrderBy(o => o.Name)
                .ToListAsync();
            return results.ToImmutableList();
        }

        public async Task<IReadOnlyList<Organisation>> ListByUserId(Guid userId)
        {
            var results = await _context.Organisations
                .Where(o => o.Users.Any(x => x.Id == userId))
                .ToListAsync();
            return results.ToImmutableList();
        }
    }
}
