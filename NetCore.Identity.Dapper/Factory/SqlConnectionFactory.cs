using NetCore.Identity.Dapper.Interfaces;
using System;
using System.Data;
using System.Data.SqlClient;

namespace NetCore.Identity.Dapper.Factory
{
    internal class SqlConnectionFactory : IDatabaseConnectionFactory
    {
        private string _connectionString;

        public SqlConnectionFactory(string connectionString, string schema)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }
            _connectionString = connectionString;
            DbSchema = schema.Replace("[", string.Empty).Replace("]", string.Empty);
        }

        public string DbSchema { get; set; }

        public IDbConnection CreateConnection()
        {
            var sqlConnection = new SqlConnection(_connectionString);
            return sqlConnection;
        }
    }
}
