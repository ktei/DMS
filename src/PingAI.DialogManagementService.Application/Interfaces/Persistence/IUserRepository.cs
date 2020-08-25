using System;
using System.Linq;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Interfaces.Persistence
{
    public interface IUserRepository
    {
        Task<User?> GetUserByAut0Id(string auth0Id, Func<IQueryable<User>, IQueryable<User>>? configureQuery = default);
    }
}