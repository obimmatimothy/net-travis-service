using System;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using Serilog;
using TravisService.Adapters.Sns.Infrastructure;
using TravisService.Adapters.Sns.Ports;
using TravisService.Exceptions;
using TravisService.Infrastructure;
using TravisService.Models;
using TravisService.Tests.Shared.AutoFixtureAttributes;
using Xunit;

namespace TravisService.Adapters.Sns.Tests.Unit.Ports
{
    public class SnsNotificationServiceTests
    {
        private readonly Fixture _fixture = new();
        private readonly SnsSettings _snsSettings = new();
        private readonly Mock<IAmazonSimpleNotificationService> _amazonService = new();
        private readonly SnsNotificationService _notificationService;

        public SnsNotificationServiceTests()
        {
            var logger = _fixture.Create<LoggerConfiguration>().CreateLogger();
            var metrics = _fixture.Create<Metrics>();
            var monitoringEvents = new MonitoringEvents(logger, metrics);

            _notificationService = new(_amazonService.Object, _snsSettings, monitoringEvents);
        }

        [Theory, AutoMoqData]
        internal void ConstructorIsGuardedAgainstNullValues(GuardClauseAssertion assertion) =>
            assertion.Verify(typeof(SnsNotificationService).GetConstructors());

        [Theory, AutoData]
        internal async Task GivenAmazonConnectionThrowsAnException_WhenPublishingAlert_CustomExceptionIsThrown(AlertEvent alertEvent, Exception exception)
        {
            _amazonService.Setup(a => a.PublishAsync(It.IsAny<PublishRequest>(), default)).ThrowsAsync(exception);

            Func<Task> action = () => _notificationService.PublishAlertAsync(alertEvent);

            await action.Should().ThrowExactlyAsync<FailedToPublishAlertException>();
        }
    }
}
