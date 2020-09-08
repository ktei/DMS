using System;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Interfaces.Persistence
{
    public interface IUserRepository
    {
        Task<User?> GetUserByAuth0Id(string auth0Id);
        Task<User?> GetUserById(Guid userId);
    }
}