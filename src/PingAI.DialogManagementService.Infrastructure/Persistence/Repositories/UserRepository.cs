using System;
using System.Collections.Generic;
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

        public async Task<User?> GetUserByAuth0Id(string auth0Id)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Include(x => x.Organisations)
                .FirstOrDefaultAsync(x => x.Auth0Id == auth0Id);
            return user;
        }

        public async Task<User?> GetUserById(Guid userId)
        {
            var user = await _context.Users
                .Include(x => x.Organisations)
                .FirstOrDefaultAsync(x => x.Id == userId);
            return user;
        }

        public async Task<User?> GetUserByName(string name)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Name == name);
            return user;
        }

        public async Task<User> AddUser(User user)
        {
            var result = await _context.Users.AddAsync(user);
            return result.Entity;
        }

        public Task<List<User>> GetAllUsers()
        {
            return _context.Users.ToListAsync();
        }
    }
}