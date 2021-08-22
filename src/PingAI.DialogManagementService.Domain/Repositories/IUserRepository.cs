using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User?> FindByAuth0Id(string auth0Id);
        Task<User> Add(User user);
        Task<List<User>> ListAll();
    }
}