using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PingAI.DialogManagementService.Api.IntegrationTests.Utils;
using PingAI.DialogManagementService.Application.Interfaces.Services.Caching;
using Xunit;
using static Xunit.Assert;

namespace PingAI.DialogManagementService.Api.IntegrationTests.Services
{
    public class CacheServiceTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly TestWebApplicationFactory _factory;

        public CacheServiceTests(TestWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task SetGetObject()
        {
            var sut = CreateSut();
            const string key = "__foo__";

            await sut.SetObject(key, new Foo());
            var actual = await sut.GetObject<Foo>(key);
            
            NotNull(actual);
            Equal("Bar", actual!.Bar);
        }

        [Fact]
        public async Task SetGetString()
        {
            var sut = CreateSut();
            const string key = "__foo__";

            await sut.SetString(key, "foo");
            var actual = await sut.GetString(key);
            
            Equal("foo", actual);
        }

        private ICacheService CreateSut()
        {
            var scope = _factory.Services.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<ICacheService>();
            return sut;
        }

        private class Foo
        {
            public string Bar { get; set; } = "Bar";
        }
    }
}