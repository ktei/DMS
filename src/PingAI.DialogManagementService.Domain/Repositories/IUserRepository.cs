using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByAuth0Id(string auth0Id);
        Task<User?> GetUserById(Guid userId);
        Task<User?> GetUserByName(string name);
        Task<User> AddUser(User user);
        Task<List<User>> GetAllUsers();
    }
}