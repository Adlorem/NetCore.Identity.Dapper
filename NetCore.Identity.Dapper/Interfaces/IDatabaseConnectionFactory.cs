using System.Data;
using System.Data.SqlClient;

namespace NetCore.Identity.Dapper.Interfaces
{
    /// <summary>
    /// Database connection factory.
    /// </summary>
    public interface IDatabaseConnectionFactory
    {
        IDbConnection CreateConnection();
        string DbSchema { get; set; }
    }
}