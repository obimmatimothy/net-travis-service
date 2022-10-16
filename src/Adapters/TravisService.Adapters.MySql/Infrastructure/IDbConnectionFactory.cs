using System.Data;
using System.Threading.Tasks;

namespace TravisService.Adapters.MySql.Infrastructure
{
    internal interface IDbConnectionFactory
    {
        Task<IDbConnection> CreateDbConnection();
    }
}
