using System.Threading.Tasks;
using TravisService.Models;

namespace TravisService.Calculators.Interfaces
{
    internal interface IDistanceCalculator
    {
        Task<double> CalculateDistance(LocationCoordinate initialLocation, LocationCoordinate latterLocation);
    }
}
