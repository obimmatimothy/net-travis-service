using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using Serilog;
using TravisService.Adapters.MySql.Infrastructure;
using TravisService.Adapters.MySql.Ports;
using TravisService.Exceptions;
using TravisService.Infrastructure;
using TravisService.Models;
using TravisService.Tests.Shared.AutoFixtureAttributes;
using Xunit;

namespace TravisService.Adapters.MySql.Tests.Unit.Ports
{
    public class MySqlHandheldEventStoreTests
    {
        private readonly Mock<IDbConnectionFactory> _connectionFactoryMock = new();
        private readonly Fixture _fixture = new();
        private readonly MySqlHandheldEventStore _eventStore;

        public MySqlHandheldEventStoreTests()
        {
            var logger = _fixture.Create<LoggerConfiguration>().CreateLogger();
            var metrics = _fixture.Create<Metrics>();
            var monitoringEvents = new MonitoringEvents(logger, metrics);

            _eventStore = new(_connectionFactoryMock.Object, monitoringEvents);
        }

        [Theory, AutoMoqData]
        internal void ConstructorIsGuardedAgainstNullValues(GuardClauseAssertion assertion) =>
            assertion.Verify(typeof(MySqlHandheldEventStore).GetConstructors());

        [Theory, AutoData]
        internal async Task GivenMySqlConnectionThrowsAnException_WhenExecuting_CustomExceptionIsThrown(
            IReadOnlyCollection<HandheldEvent> handheldEvents,
            Exception exception)
        {
            _connectionFactoryMock.Setup(c => c.CreateDbConnection()).ThrowsAsync(exception);

            Func<Task> action = () => _eventStore.StoreEventsAsync(handheldEvents);

            await action.Should().ThrowExactlyAsync<UnableToStoreHandheldEventsException>();
        }
    }
}
