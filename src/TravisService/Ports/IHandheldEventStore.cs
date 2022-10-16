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
    }
}
