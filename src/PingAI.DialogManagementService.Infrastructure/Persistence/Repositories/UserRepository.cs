using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DialogManagementContext _context;

        public UserRepository(DialogManagementContext context)
        {
            _context = context;
        }

        public Task<User?> GetUserByAuth0Id(string auth0Id,
            Func<IQueryable<User>, IQueryable<User>>? configureUser = default)
        {
            IQueryable<User> query = _context.Users
                .AsNoTracking()
                .Include(x => x.OrganisationUsers);

            query = configureUser?.Invoke(query) ?? query;
            return query.FirstOrDefaultAsync(x => x.Auth0Id == auth0Id);
        }
    }
}