using System;
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

        public Task<User?> GetUserByAut0Id(string auth0Id)
        {
            return _context.Users
                .Include(x => x.OrganisationUsers)
                .FirstOrDefaultAsync(x => x.Auth0Id == auth0Id);
        }
    }
}