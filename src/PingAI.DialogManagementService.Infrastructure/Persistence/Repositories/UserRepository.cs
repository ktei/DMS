using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Domain.Repositories;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DialogManagementContext _context;

        public UserRepository(DialogManagementContext context)
        {
            _context = context;
        }

        public async Task<User?> FindByAuth0Id(string auth0Id)
        {
            var user = await _context.Users
                .Include(x => x.Organisations)
                .SingleOrDefaultAsync(x => x.Auth0Id == auth0Id);
            return user;
        }

        public async Task<User> Add(User user)
        {
            var result = await _context.Users.AddAsync(user);
            return result.Entity;
        }

        public Task<List<User>> ListAll()
        {
            return _context.Users.ToListAsync();
        }
    }
}