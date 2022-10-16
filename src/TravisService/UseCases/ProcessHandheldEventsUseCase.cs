using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TravisService.Exceptions;
using TravisService.Infrastructure;
using TravisService.Models;
using TravisService.Ports;

namespace TravisService.UseCases
{
    internal sealed class ProcessHandheldEventsUseCase : IProcessHandheldEventsUseCase
    {
        private readonly IHandheldEventStore _handheldEventsStore;
        private readonly MonitoringEvents _monitoringEvents;

        public ProcessHandheldEventsUseCase(IHandheldEventStore handheldEventsStore, MonitoringEvents monitoringEvents)
        {
            _handheldEventsStore = handheldEventsStore ?? throw new ArgumentNullException(nameof(handheldEventsStore));
            _monitoringEvents = monitoringEvents ?? throw new ArgumentNullException(nameof(monitoringEvents));
        }

        public async Task ExecuteAsync(IReadOnlyCollection<HandheldEvent> handheldEvents)
        {
            _ = handheldEvents ?? throw new ArgumentNullException(nameof(handheldEvents));

            try
            {
                await _handheldEventsStore.StoreEventsAsync(handheldEvents);
            }
            catch (Exception exception)
            {
                _monitoringEvents.Logger.Error("An error occured while processing handheld events");
                throw new FailedToProcessHandheldEventsException(exception);
            }
        }
    }
}
