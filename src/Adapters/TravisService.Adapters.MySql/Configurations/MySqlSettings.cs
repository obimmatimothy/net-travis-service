using System.Data.Common;

namespace TravisService.Adapters.MySql.Configurations
{
    public sealed class MySqlSettings
    {
        private string _connectionString;

        public string Password { get; set; }

        public string ConnectionString
        {
            get => GetConnectionString(_connectionString);
            set => _connectionString = value;
        }

        private string GetConnectionString(string baseConnectionString)
        {
            var builder = new DbConnectionStringBuilder() { ConnectionString = baseConnectionString };

            if (Password is not null)
            {
                builder.Add("Password", Password);
            }

            return builder.ToString();
        }
    }
}
