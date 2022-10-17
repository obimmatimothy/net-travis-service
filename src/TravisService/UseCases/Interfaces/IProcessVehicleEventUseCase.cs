using System.Threading.Tasks;
using TravisService.Exceptions;
using TravisService.Models;

namespace TravisService.UseCases
{
    public interface IProcessVehicleEventUseCase
    {
        /// <exception cref="FailedToProcessVehicheEventException"></exception>
        /// <exception cref="NoHandheldEventsFoundException"></exception>
        Task ExecuteAsync(VehicleEvent vehicleEvent);
    }
}
