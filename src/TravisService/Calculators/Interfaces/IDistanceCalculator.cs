using System.Threading.Tasks;
using TravisService.Models;

namespace TravisService.Calculators
{
    internal interface IDistanceCalculator
    {
        Task<double> CalculateDistance(LocationCoordinate initialLocation, LocationCoordinate latterLocation);
    }
}
