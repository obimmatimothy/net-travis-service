using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TravisService.Exceptions;
using TravisService.Models;

namespace TravisService.Ports
{
    public interface IHandheldEventStore
    {
        /// <summary>
        /// Stores a collection of handheld events in a data store
        /// </summary>
        /// <exception cref="UnableToStoreHandheldEventsException"></exception>
        Task StoreEventsAsync(IReadOnlyCollection<HandheldEvent> handheldEvents);

        /// <summary>
        /// Retrieves a collection of events for a handheld device within the provided timeframe
        /// </summary>
        /// <exception cref="UnableToRetrieveHandheldEventsException"></exception>
        Task<IReadOnlyCollection<HandheldEvent>> GetEventsAsync(string handheldId, DateTime from, DateTime to);
    }
}
