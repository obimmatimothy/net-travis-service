using System;
using System.Threading.Tasks;
using TravisService.Models;

namespace TravisService.Calculators
{
    internal sealed class DistanceCalculator : IDistanceCalculator
    {
        /// <remarks>
        /// Was expecting to use System.Device.Location to calculate these distances but that library only exists in .NET framework
        /// Would be nie to have some library do this but for now, this is some random code from stackoverflow :(
        /// </remarks>
        public Task<double> CalculateDistance(LocationCoordinate initialLocation, LocationCoordinate latterLocation)
        {

            var d1 = initialLocation.Latitude * (Math.PI / 180.0);
            var num1 = initialLocation.Longitude * (Math.PI / 180.0);

            var d2 = latterLocation.Latitude * (Math.PI / 180.0);
            var num2 = latterLocation.Longitude * (Math.PI / 180.0) - num1;

            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
                     Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

            var result = 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));

            return Task.FromResult(result);
        }
    }
}
