using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using Serilog;
using TravisService.Exceptions;
using TravisService.Infrastructure;
using TravisService.Models;
using TravisService.Ports;
using TravisService.Tests.Shared.AutoFixtureAttributes;
using TravisService.UseCases;
using Xunit;

namespace TravisService.Tests.Unit.UseCases
{
    public  class ProcessHandheldEventsUseCaseTests
    {
        private readonly Mock<IHandheldEventStore> _handheldEventStore = new();
        private readonly Fixture _fixture = new();
        private readonly ProcessHandheldEventsUseCase _useCase;

        public ProcessHandheldEventsUseCaseTests()
        {
            var logger = _fixture.Create<LoggerConfiguration>().CreateLogger();
            var metrics = _fixture.Create<Metrics>();
            var monitoringEvents = new MonitoringEvents(logger, metrics);

            _useCase = new(_handheldEventStore.Object, monitoringEvents);
        }

        [Theory, AutoMoqData]
        internal void ConstructorIsGuardedAgainstNullValues(GuardClauseAssertion assertion) =>
            assertion.Verify(typeof(ProcessHandheldEventsUseCase).GetConstructors());

        [Fact]
        internal async Task GivenANullArgument_WhenExecutingUseCase_ArgumentNullExceptionIsThrown()
        {
            Func<Task> action = () => _useCase.ExecuteAsync(null);

            await action.Should().ThrowExactlyAsync<ArgumentNullException>();
        }

        [Theory, AutoData]
        internal async Task GivenHandheldEventStoreThrowsAnException_WhenExecuting_CustomExceptionIsThrown(
            IReadOnlyCollection<HandheldEvent> handheldEvents,
            UnableToStoreHandheldEventsException exception)
        {
            _handheldEventStore.Setup(c => c.StoreEventsAsync(handheldEvents)).ThrowsAsync(exception);

            Func<Task> action = () => _useCase.ExecuteAsync(handheldEvents);

            await action.Should().ThrowExactlyAsync<FailedToProcessHandheldEventsException>();
        }
    }
}
