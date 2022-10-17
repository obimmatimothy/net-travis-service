using System;
using System.Threading.Tasks;
using TravisService.Models;

namespace TravisService.UseCases
{
    public sealed class ProcessVehicleEventUseCase : IProcessVehicleEventUseCase
    {
        public Task ExecuteAsync(VehicleEvent vehicleEvent) => throw new NotImplementedException();
    }
}
