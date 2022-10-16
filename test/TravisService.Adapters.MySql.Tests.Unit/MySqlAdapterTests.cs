using System;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using TravisService.Adapters.MySql.Configurations;
using TravisService.Adapters.MySql.Infrastructure;
using TravisService.Adapters.MySql.Ports;
using TravisService.Infrastructure;
using TravisService.Ports;
using Xunit;

namespace TravisService.Adapters.MySql.Tests.Unit
{
    public class MySqlAdapterTests
    {
        private readonly Fixture _fixture = new();

        [Fact]
        public void GivenAContainer_WhenAddingMySqlAdapter_IHandheldEventStoreIsRegisteredAsExpected()
        {
            var sut = SetupContainer();

            var service = sut.GetService<IHandheldEventStore>();

            service.Should().NotBeNull();
            service.Should().BeOfType<MySqlHandheldEventStore>();
        }

        [Fact]
        public void GivenAContainer_WhenAddingMySqlAdapter_IDbConnectionFactoryIsRegisteredAsExpected()
        {
            var sut = SetupContainer();

            var service = sut.GetService<IDbConnectionFactory>();

            service.Should().NotBeNull();
            service.Should().BeOfType<MySqlConnectionFactory>();
        }

        private IServiceProvider SetupContainer()
        {
            var services = new ServiceCollection();

            var settings = new MySqlSettings();

            var logger = _fixture.Create<LoggerConfiguration>().CreateLogger();
            var metrics = _fixture.Create<Metrics>();
            var monitoringEvents = new MonitoringEvents(logger, metrics);

            services.AddMySqlAdapter(settings, monitoringEvents);

            return services.BuildServiceProvider();
        }
    }
}
