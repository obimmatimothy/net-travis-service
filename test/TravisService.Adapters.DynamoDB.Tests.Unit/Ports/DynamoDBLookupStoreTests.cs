using System;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using Serilog;
using TravisService.Adapters.DynamoDB.Infrastructure;
using TravisService.Adapters.DynamoDB.Ports;
using TravisService.Exceptions;
using TravisService.Infrastructure;
using TravisService.Tests.Shared.AutoFixtureAttributes;
using Xunit;

namespace TravisService.Adapters.DynamoDB.Tests.Unit.Ports
{
    public class DynamoDBLookupStoreTests
    {
        private readonly Mock<IAmazonDynamoDB> _dynamoDBClient = new();
        private readonly DynamoDBSettings settings = new() { AwsRegion = "eu-west-1", Profile = "development" };
        private readonly Fixture _fixture = new();
        private readonly DynamoDBLookupStore _lookupStore;

        public DynamoDBLookupStoreTests()
        {
            var logger = _fixture.Create<LoggerConfiguration>().CreateLogger();
            var metrics = _fixture.Create<Metrics>();
            var monitoringEvents = new MonitoringEvents(logger, metrics);

            _lookupStore = new(_dynamoDBClient.Object, settings, monitoringEvents);
        }

        [Theory, AutoMoqData]
        internal void ConstructorIsGuardedAgainstNullValues(GuardClauseAssertion assertion) =>
            assertion.Verify(typeof(DynamoDBLookupStore).GetConstructors());

        [Theory, AutoData]
        internal async Task GiveAnExceptionIsThrownWhileRetrievingAMessage_CustomExceptionIsThrown(string vehicleId)
        {
            _dynamoDBClient.Setup(d => d.GetItemAsync(It.IsAny<GetItemRequest>(), default)).ThrowsAsync(new Exception());

            Func<Task> action = async () => await _lookupStore.GetHandheldIdAsync(vehicleId);

            await action.Should().ThrowExactlyAsync<UnableToRetrieveHandheldException>();
        }
    }
}
