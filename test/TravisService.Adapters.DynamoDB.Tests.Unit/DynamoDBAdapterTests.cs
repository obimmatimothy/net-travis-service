using System;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using TravisService.Adapters.DynamoDB.Infrastructure;
using TravisService.Adapters.DynamoDB.Ports;
using TravisService.Infrastructure;
using TravisService.Ports;
using Xunit;

namespace TravisService.Adapters.DynamoDB.Tests.Unit
{
    public class DynamoDBAdapterTests
    {
        private readonly Fixture _fixture = new();

        [Fact]
        public void GivenAContainer_WhenAddingMySqlAdapter_IHandheldEventStoreIsRegisteredAsExpected()
        {
            var sut = SetupContainer();

            var service = sut.GetService<ILookupStore>();

            service.Should().NotBeNull();
            service.Should().BeOfType<DynamoDBLookupStore>();
        }

        private IServiceProvider SetupContainer()
        {
            var services = new ServiceCollection();

            var settings = new DynamoDBSettings { AwsRegion = "eu-west-1", Profile = "development" };
            var logger = _fixture.Create<LoggerConfiguration>().CreateLogger();
            var metrics = _fixture.Create<Metrics>();
            var monitoringEvents = new MonitoringEvents(logger, metrics);

            services.AddDynamoDBAdapter(settings, monitoringEvents);

            return services.BuildServiceProvider();
        }
    }
}
