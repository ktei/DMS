using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Infrastructure.Persistence;

namespace PingAI.DialogManagementService.Api.IntegrationTests.Utils
{
    public class TestUserIdProvider
    {
        private readonly Guid? _userId;
        private TestUserIdProvider(Guid? userId)
        {
            _userId = userId;
        }

        public Guid? GetUserId() => _userId;

        public static TestUserIdProvider FirstUser => new TestUserIdProvider(null);
        public static TestUserIdProvider UseId(Guid id) => new TestUserIdProvider(id);
    }
    
    public class TestUserFinder
    {
        private readonly TestUserIdProvider _userIdProvider;
        private readonly DialogManagementContext _context;

        public TestUserFinder(DialogManagementContext context, TestUserIdProvider userIdProvider)
        {
            _context = context;
            _userIdProvider = userIdProvider;
        }

        public Task<User> FindUser()
        {
            var id = _userIdProvider.GetUserId();
            if (id == null)
                return _context.Users.FirstAsync();
            return _context.Users.SingleAsync(x => x.Id == id);
        }
    }
}
