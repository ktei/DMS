using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;

namespace PingAI.DialogManagementService.TestingUtil.AutoMoq
{
    public static class FixtureFactory
    {
        public static T CreateSut<T>(Action<IFixture>? configureFixture = null) where T : class
        {
            var fixture = new Fixture();
            fixture.Customize(new AutoMoqCustomization {ConfigureMembers = true});
            configureFixture?.Invoke(fixture);
            return fixture.Create<T>();
        }

        public static Mock<T> InjectMock<T>(this IFixture fixture, Action<Mock<T>>? configureMock = null) where T : class
        {
            var mock = new Mock<T>();
            configureMock?.Invoke(mock);
            fixture.Inject(mock);
            return mock;
        }
    }
}
