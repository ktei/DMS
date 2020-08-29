using PingAI.DialogManagementService.Domain.Model;
using Xunit;
using static Xunit.Assert;

namespace PingAI.DialogManagementService.Domain.UnitTests.ValueObjects
{
    public class ApiKeyTests
    {
        [Fact]
        public void GenerateNew()
        {
            var key1 = ApiKey.GenerateNew();
            var key2 = ApiKey.GenerateNew();
            
            NotEmpty(key1.Key);
            NotEmpty(key2.Key);
            NotEqual(key1, key2);
        }
    }
}