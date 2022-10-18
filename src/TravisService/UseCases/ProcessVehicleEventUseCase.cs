using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravisService.Calculators;
using TravisService.Exceptions;
using TravisService.Extensions;
using TravisService.Infrastructure;
using TravisService.Models;
using TravisService.Ports;

namespace TravisService.UseCases
{
    public sealed class ProcessVehicleEventUseCase : IProcessVehicleEventUseCase
    {
        private const int AlertDistance = 50;
        private const int EventsFrequencyInSeconds = 15;

        private readonly ILookupStore _lookupStore;
        private readonly IHandheldEventStore _handheldEventStore;
        private readonly IDistanceCalculator _distanceCalculator;
        private readonly INotificationService _notificationService;
        private readonly MonitoringEvents _monitoringEvents;

        public ProcessVehicleEventUseCase(ILookupStore lookupStore,
            IHandheldEventStore handheldEventStore,
            IDistanceCalculator distanceCalculator,
            INotificationService notificationService,
            MonitoringEvents monitoringEvents)
        {
            _lookupStore = lookupStore ?? throw new ArgumentNullException(nameof(lookupStore));
            _handheldEventStore = handheldEventStore ?? throw new ArgumentNullException(nameof(handheldEventStore));
            _distanceCalculator = distanceCalculator ?? throw new ArgumentNullException(nameof(distanceCalculator));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _monitoringEvents = monitoringEvents ?? throw new ArgumentNullException(nameof(monitoringEvents));
        }

        public async Task ExecuteAsync(VehicleEvent vehicleEvent)
        {
            _ = vehicleEvent ?? throw new ArgumentNullException(nameof(vehicleEvent));

            try
            {
                var handheldId = await _lookupStore.GetHandheldIdAsync(vehicleEvent.VehicleId);

                if (string.IsNullOrWhiteSpace(handheldId))
                {
                    _monitoringEvents.NoCorrespondingHandheld(vehicleEvent.VehicleId);
                    return;
                }

                var from = vehicleEvent.Timestamp.AddSeconds(-EventsFrequencyInSeconds);
                var to = vehicleEvent.Timestamp.AddSeconds(EventsFrequencyInSeconds);

                var handheldEvents = await _handheldEventStore.GetEventsAsync(handheldId, from, to);

                if (await ShouldSendAlert(handheldEvents, vehicleEvent))
                {
                    var alert = new AlertEvent(handheldId, vehicleEvent.VehicleId, vehicleEvent.Longitude, vehicleEvent.Latitude);

                    await _notificationService.PublishAlertAsync(alert);
                }
            }
            catch (NoHandheldEventsFoundException)
            {
                //We want to give another chance with the hope that more handheld events come in
                _monitoringEvents.NotEnoughHandheldEvents(vehicleEvent.VehicleId, vehicleEvent.Timestamp);
                throw;
            }
            catch (Exception exception)
            {
                _monitoringEvents.ExceptionOccured(exception, vehicleEvent.VehicleId);
                throw new FailedToProcessVehicheEventException(exception);
            }
        }

        private async Task<bool> ShouldSendAlert(IReadOnlyCollection<HandheldEvent> handheldEvents, VehicleEvent vehicleEvent) =>
            handheldEvents is null || handheldEvents.Count <= 1
                ? throw new NoHandheldEventsFoundException()
                : await CalculateDistanceBetweenVehicleAndHandheld(handheldEvents, vehicleEvent) >= AlertDistance;

        private async Task<double> CalculateDistanceBetweenVehicleAndHandheld(IReadOnlyCollection<HandheldEvent> handheldEvents, VehicleEvent vehicleEvent)
        {
            var initialHandheldEvent = handheldEvents.First();
            var latterHandheldEvent = handheldEvents.Last();
            var distanceBetweenHandheldEvents =
                await _distanceCalculator.CalculateDistance(initialHandheldEvent.GetLocationCoordinate(), latterHandheldEvent.GetLocationCoordinate());

            var averagePacePerSecond = distanceBetweenHandheldEvents / (initialHandheldEvent.Timestamp - latterHandheldEvent.Timestamp).TotalSeconds;

            var distanceBetweenVehicleAndHandheld =
                await _distanceCalculator.CalculateDistance(initialHandheldEvent.GetLocationCoordinate(), vehicleEvent.GetLocationCoordinate());
            var timeBetweenVehicleAndHandheldEventsInSeconds = (initialHandheldEvent.Timestamp - vehicleEvent.Timestamp).TotalSeconds;

            return distanceBetweenVehicleAndHandheld - (timeBetweenVehicleAndHandheldEventsInSeconds * averagePacePerSecond);
        }
    }
}
