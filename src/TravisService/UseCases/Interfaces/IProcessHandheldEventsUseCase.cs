using System.Collections.Generic;
using System.Threading.Tasks;
using TravisService.Exceptions;
using TravisService.Models;

namespace TravisService.UseCases
{
    public interface IProcessHandheldEventsUseCase
    {
        /// <exception cref="FailedToProcessHandheldEventsException"></exception>
        Task ExecuteAsync(IReadOnlyCollection<HandheldEvent> handheldEvents);
    }
}
