using System;
using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using TravisService.Adapters.MySql.Configurations;

namespace TravisService.Adapters.MySql.Infrastructure
{
    internal sealed class MySqlConnectionFactory : IDbConnectionFactory
    {
        private readonly MySqlConnection _mySqlConnection;

        public MySqlConnectionFactory(MySqlSettings mySqlSettings)
        {
            mySqlSettings = mySqlSettings ?? throw new ArgumentNullException(nameof(mySqlSettings));
            _mySqlConnection = new(mySqlSettings.ConnectionString);
        }

        public async Task<IDbConnection> CreateDbConnection()
        {
            await _mySqlConnection.OpenAsync();

            return _mySqlConnection;
        }
    }
}
