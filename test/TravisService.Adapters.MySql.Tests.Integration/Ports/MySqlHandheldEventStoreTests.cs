using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Xunit2;
using Dapper;
using FluentAssertions;
using Serilog;
using TravisService.Adapters.MySql.Configurations;
using TravisService.Adapters.MySql.Infrastructure;
using TravisService.Adapters.MySql.Models;
using TravisService.Adapters.MySql.Ports;
using TravisService.Infrastructure;
using TravisService.Models;
using Xunit;

namespace TravisService.Adapters.MySql.Tests.Acceptance.Ports
{
    public class MySqlHandheldEventStoreTests
    {
        private readonly MySqlSettings _settings = new()
        {
            ConnectionString = "Server=localhost;Port=3396;Database=travisservice;Uid=root;SslMode=None;minpoolsize=3;maxpoolsize=40;pooling=true;ConnectionLifeTime=600;Connect Timeout=10",
            Password = "root",
        };

        private readonly MySqlConnectionFactory _connectionFactory;
        private readonly Fixture _fixture = new();
        private readonly MySqlHandheldEventStore _eventStore;

        public MySqlHandheldEventStoreTests()
        {
            var logger = _fixture.Create<LoggerConfiguration>().CreateLogger();
            var metrics = _fixture.Create<Metrics>();
            var monitoringEvents = new MonitoringEvents(logger, metrics);

            _connectionFactory = new(_settings);

            _eventStore = new(_connectionFactory, monitoringEvents);
        }

        [Theory, AutoData]
        internal async Task GivenACollectionOfHandheldEvents_WhenExecuting_EventsAreStoredSuccessfully(IReadOnlyCollection<HandheldEvent> handheldEvents)
        {
            await GivenAFreshDatabase();

            await _eventStore.StoreEventsAsync(handheldEvents);

            var events = await GetAllEvents();

            events.Count.Should().Be(handheldEvents.Count);
        }

        private async Task GivenAFreshDatabase()
        {
            using var connection = await _connectionFactory.CreateDbConnection();

            await connection.ExecuteAsync("delete from HandheldEvents");
        }

        private async Task<IReadOnlyCollection<HandheldEventStorageModel>> GetAllEvents()
        {
            using var connection = await _connectionFactory.CreateDbConnection();

            var result = await connection.QueryAsync<HandheldEventStorageModel>("select HandheldId, Longitude, Latitude, Timestamp from HandheldEvents");

            return result.ToList();
        }

    }
}
