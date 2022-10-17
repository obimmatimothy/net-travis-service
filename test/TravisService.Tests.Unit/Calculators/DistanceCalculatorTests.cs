using System.Threading.Tasks;
using FluentAssertions;
using TravisService.Calculators;
using TravisService.Models;
using Xunit;

namespace TravisService.Tests.Unit.Calculators
{
    public class DistanceCalculatorTests
    {
        private readonly DistanceCalculator _calculator = new();

        [Theory]
        [InlineData(4.816585062586511, 52.44258347573056, 4.818172930278721, 52.44406145203452, 196.61)]
        internal async Task GivenASetOfCoordinates_WhenCalculating_TheRightDistanceIsReturned(
            double long1,
            double lat1,
            double long2,
            double lat2,
            double expectedResult)
        {
            var initialLocation = new LocationCoordinate(long1, lat1);
            var latterLocation = new LocationCoordinate(long2, lat2);

            var result = await _calculator.CalculateDistance(initialLocation, latterLocation);

            result.Should().BeApproximately(expectedResult, 2);
        }
    }
}
