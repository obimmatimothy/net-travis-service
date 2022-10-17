using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SandyBridge.Infrastructure;
using TravisService.Adapters.MySql.Extensions;
using TravisService.Adapters.MySql.Infrastructure;
using TravisService.Adapters.MySql.Models;
using TravisService.Exceptions;
using TravisService.Infrastructure;
using TravisService.Models;
using TravisService.Ports;

namespace TravisService.Adapters.MySql.Ports
{
    internal sealed class MySqlHandheldEventStore : IHandheldEventStore
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly MonitoringEvents _monitoringvents;

        public MySqlHandheldEventStore(IDbConnectionFactory dbConnectionFactory, MonitoringEvents monitoringEvents)
        {
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            _monitoringvents = monitoringEvents ?? throw new ArgumentNullException(nameof(monitoringEvents));
        }

        public async Task<IReadOnlyCollection<HandheldEvent>> GetEventsAsync(string handheldId, DateTime from, DateTime to)
        {
            var query = @"
select HandheldId, Longitude, Latitude, Timestamp
from HandheldEvents
where HandheldId = @handheldId
and Timestamp >= @from
and Timestamp <= @to";

            var parameters = new
            {
                handheldId = handheldId,
                from = from,
                to = to,
            };

            try
            {
                return await Run.TimedAsync(
                    () => GetEvents(query, parameters),
                    (success, duration) => _monitoringvents.FetchHandheldEvents(success, duration));
            }
            catch (Exception exception)
            {
                _monitoringvents.FailedToGetHandheldEvents(exception);
                throw new UnableToRetrieveHandheldEventsException(exception);
            }
        }

        public async Task StoreEventsAsync(IReadOnlyCollection<HandheldEvent> handheldEvents)
        {
            var query = @"
insert into `HandheldEvents` (HandheldId, Longitude, Latitude, Timestamp)
values (@handheldId, @longitude, @latitude, @timestamp)";
            var events = handheldEvents.Select(x => HandheldEventStorageModel.FromDomain(x)).ToList();

            try
            {
                await Run.TimedAsync(
                    () => SaveEvents(events, query),
                    (success, duration) => _monitoringvents.InsertedNewHandheldEvents(success, duration));
            }
            catch (Exception exception)
            {
                _monitoringvents.FailedToSaveHandheldEvents(exception);
                throw new UnableToStoreHandheldEventsException(exception);
            }
        }

        private async Task<int> SaveEvents(IReadOnlyCollection<HandheldEventStorageModel> events, string query)
        {
            using var connection = await _dbConnectionFactory.CreateDbConnection();
            return await connection.ExecuteAsync(query, events);
        }

        private async Task<IReadOnlyCollection<HandheldEvent>> GetEvents(string query, object parameters)
        {
            using var connection = await _dbConnectionFactory.CreateDbConnection();
            var result = await connection.QueryAsync<HandheldEventStorageModel>(query, parameters);

            return result.Select(r => r.ToDomain()).ToList();
        }
    }
}
