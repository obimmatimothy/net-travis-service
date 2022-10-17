using System;
using Amazon.SimpleNotificationService;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using TravisService.Adapters.Sns.Infrastructure;
using TravisService.Adapters.Sns.Ports;
using TravisService.Infrastructure;
using TravisService.Ports;
using Xunit;

namespace TravisService.Adapters.Sns.Tests.Unit
{
    public class SnsAdapterTests
    {
        private readonly Fixture _fixture = new();

        [Fact]
        public void GivenAContainer_WhenAddingMySqlAdapter_IAmazonSimpleNotificationServiceIsRegisteredAsExpected()
        {
            var sut = SetupContainer();

            var service = sut.GetService<IAmazonSimpleNotificationService>();

            service.Should().NotBeNull();
        }

        [Fact]
        public void GivenAContainer_WhenAddingMySqlAdapter_IDbConnectionFactoryIsRegisteredAsExpected()
        {
            var sut = SetupContainer();

            var service = sut.GetService<INotificationService>();

            service.Should().NotBeNull();
            service.Should().BeOfType<SnsNotificationService>();
        }

        private IServiceProvider SetupContainer()
        {
            var services = new ServiceCollection();

            var settings = new SnsSettings { TopicArn = "Some Arn" };

            var logger = _fixture.Create<LoggerConfiguration>().CreateLogger();
            var metrics = _fixture.Create<Metrics>();
            var monitoringEvents = new MonitoringEvents(logger, metrics);

            services.AddSnsAdapter(settings, monitoringEvents);

            return services.BuildServiceProvider();
        }
    }
}
