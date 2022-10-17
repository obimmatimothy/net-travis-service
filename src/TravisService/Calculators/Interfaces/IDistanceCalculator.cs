using System.Threading.Tasks;
using TravisService.Models;

namespace TravisService.Calculators
{
    public interface IDistanceCalculator
    {
        Task<double> CalculateDistance(LocationCoordinate initialLocation, LocationCoordinate latterLocation);
    }
}
