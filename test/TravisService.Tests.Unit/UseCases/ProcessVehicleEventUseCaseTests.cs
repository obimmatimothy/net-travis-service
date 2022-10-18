using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using Serilog;
using TravisService.Calculators;
using TravisService.Exceptions;
using TravisService.Infrastructure;
using TravisService.Models;
using TravisService.Ports;
using TravisService.Tests.Shared.AutoFixtureAttributes;
using TravisService.UseCases;
using Xunit;

namespace TravisService.Tests.Unit.UseCases
{
    public class ProcessVehicleEventUseCaseTests
    {
        private readonly Mock<IHandheldEventStore> _handheldEventStore = new();
        private readonly Mock<ILookupStore> _lookupStore = new();
        private readonly Mock<IDistanceCalculator> _distanceCalculator = new();
        private readonly Mock<INotificationService> _notificationService = new();
        private readonly Fixture _fixture = new();
        private readonly ProcessVehicleEventUseCase _useCase;

        public ProcessVehicleEventUseCaseTests()
        {
            var logger = _fixture.Create<LoggerConfiguration>().CreateLogger();
            var metrics = _fixture.Create<Metrics>();
            var monitoringEvents = new MonitoringEvents(logger, metrics);

            _useCase = new(_lookupStore.Object, _handheldEventStore.Object, _distanceCalculator.Object, _notificationService.Object, monitoringEvents);
        }

        [Theory, AutoMoqData]
        internal void ConstructorIsGuardedAgainstNullValues(GuardClauseAssertion assertion) =>
            assertion.Verify(typeof(ProcessVehicleEventUseCase).GetConstructors());

        [Fact]
        internal async Task GivenANullArgument_WhenExecutingUseCase_ArgumentNullExceptionIsThrown()
        {
            Func<Task> action = () => _useCase.ExecuteAsync(null);

            await action.Should().ThrowExactlyAsync<ArgumentNullException>();
        }

        [Theory, AutoData]
        internal async Task GivenAVehicleWithNoHandheld_WhenExecutingUseCase_OtherPortsAreNotInvoked(VehicleEvent vehicleEvent)
        {
            _lookupStore.Setup(l => l.GetHandheldIdAsync(vehicleEvent.VehicleId)).ReturnsAsync((string)null);

            await _useCase.ExecuteAsync(vehicleEvent);

            _handheldEventStore.Verify(h => h.GetEventsAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never);
            _distanceCalculator.Verify(c => c.CalculateDistance(It.IsAny<LocationCoordinate>(), It.IsAny<LocationCoordinate>()), Times.Never);
            _notificationService.Verify(n => n.PublishAlertAsync(It.IsAny<AlertEvent>()), Times.Never);
        }

        [Theory, AutoData]
        internal async Task GivenAVehicleWithAHandheldButNoEvents_WhenExecutingUseCase_CustomExceptionIsThrown(VehicleEvent vehicleEvent)
        {
            var handheldEvents = new List<HandheldEvent>();

            _lookupStore.Setup(l => l.GetHandheldIdAsync(vehicleEvent.VehicleId)).ReturnsAsync(vehicleEvent.VehicleId);
            _handheldEventStore.Setup(h => h.GetEventsAsync(vehicleEvent.VehicleId, It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(handheldEvents);

            Func<Task> action = () => _useCase.ExecuteAsync(vehicleEvent);

            await action.Should().ThrowExactlyAsync<NoHandheldEventsFoundException>();
            _notificationService.Verify(n => n.PublishAlertAsync(It.IsAny<AlertEvent>()), Times.Never);
        }

        [Theory, AutoData]
        internal async Task GivenAnExceptionIsThrownBytheLookupStore_WhenExecutingUseCase_CustomExceptionIsThrown(
            VehicleEvent vehicleEvent,
            Exception exception)
        {
            _lookupStore.Setup(l => l.GetHandheldIdAsync(vehicleEvent.VehicleId)).ThrowsAsync(exception);

            Func<Task> action = () => _useCase.ExecuteAsync(vehicleEvent);

            await action.Should().ThrowExactlyAsync<FailedToProcessVehicheEventException>();
            _handheldEventStore.Verify(h => h.GetEventsAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never);
            _distanceCalculator.Verify(c => c.CalculateDistance(It.IsAny<LocationCoordinate>(), It.IsAny<LocationCoordinate>()), Times.Never);
            _notificationService.Verify(n => n.PublishAlertAsync(It.IsAny<AlertEvent>()), Times.Never);
        }

        [Theory, AutoData]
        internal async Task GivenAVehicleWithHandheldEventsMoreThan50M_WhenExecutingUseCase_AlertIsSent(
            VehicleEvent vehicleEvent,
            IReadOnlyCollection<HandheldEvent> handheldEvents)
        {
            handheldEvents = GetHandheldEventsWithCorrectTimestamp(handheldEvents, vehicleEvent);

            _lookupStore.Setup(l => l.GetHandheldIdAsync(vehicleEvent.VehicleId)).ReturnsAsync(vehicleEvent.VehicleId);
            _handheldEventStore.Setup(h => h.GetEventsAsync(vehicleEvent.VehicleId, It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(handheldEvents);
            _distanceCalculator.Setup(c => c.CalculateDistance(It.IsAny<LocationCoordinate>(), It.IsAny<LocationCoordinate>())).ReturnsAsync(200);

            await _useCase.ExecuteAsync(vehicleEvent);

            _notificationService.Verify(n => n.PublishAlertAsync(It.IsAny<AlertEvent>()), Times.Once);
        }

        [Theory, AutoData]
        internal async Task GivenAVehicleWithHandheldEventsLessThan50M_WhenExecutingUseCase_AlertIsNotSent(
            VehicleEvent vehicleEvent,
            IReadOnlyCollection<HandheldEvent> handheldEvents)
        {
            handheldEvents = GetHandheldEventsWithCorrectTimestamp(handheldEvents, vehicleEvent);

            _lookupStore.Setup(l => l.GetHandheldIdAsync(vehicleEvent.VehicleId)).ReturnsAsync(vehicleEvent.VehicleId);
            _handheldEventStore.Setup(h => h.GetEventsAsync(vehicleEvent.VehicleId, It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(handheldEvents);
            _distanceCalculator.Setup(c => c.CalculateDistance(It.IsAny<LocationCoordinate>(), It.IsAny<LocationCoordinate>())).ReturnsAsync(50);

            await _useCase.ExecuteAsync(vehicleEvent);

            _notificationService.Verify(n => n.PublishAlertAsync(It.IsAny<AlertEvent>()), Times.Never);
        }

        private static IReadOnlyCollection<HandheldEvent> GetHandheldEventsWithCorrectTimestamp(
            IReadOnlyCollection<HandheldEvent> handheldEvents,
            VehicleEvent vehicleEvent)
        {
            var firstHandheldEvent = handheldEvents.First();
            var lastHandheldEvent = handheldEvents.Last();
            return new List<HandheldEvent>
            {
                CreateHandheldEvent(firstHandheldEvent, vehicleEvent.Timestamp.AddSeconds(-7)),
                CreateHandheldEvent(lastHandheldEvent, vehicleEvent.Timestamp.AddSeconds(8)),
            };
        }

        private static HandheldEvent CreateHandheldEvent(HandheldEvent handheldEvent, DateTime dateTime) =>
            new(handheldEvent.HandheldId, handheldEvent.Longitude, handheldEvent.Latitude, dateTime);
    }
}
