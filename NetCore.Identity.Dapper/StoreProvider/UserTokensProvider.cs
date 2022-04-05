using NetCore.Identity.Dapper.Interfaces;
using NetCore.Identity.Dapper.Models;
using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCore.Identity.Dapper.StoreProvider
{
    internal class UserTokensProvider<TUser> where TUser : class, IDapperIdentityUser<TUser>
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        public UserTokensProvider(IDatabaseConnectionFactory databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory;
        }

        public async Task<IEnumerable<UserToken>> GetTokensAsync(string userId)
        {
            var command = $"SELECT * FROM [{_databaseConnectionFactory.DbSchema}].[AspNetUserTokens] " +
                                   "WHERE UserId = @UserId;";

            using (var sqlConnection = _databaseConnectionFactory.CreateConnection())
            {
                return await sqlConnection.QueryAsync<UserToken>(command, new
                {
                    UserId = userId
                });
            }
        }

        public async Task AddTokenAsync(TUser user, UserToken token)
        {
            var command = $"INSERT INTO [{_databaseConnectionFactory.DbSchema}].AspNetUserTokens (UserId, LoginProvider, Name, Value) " +
                                         "VALUES (@UserId, @LoginProvider, @Name, @Value);";

            using (var sqlConnection = _databaseConnectionFactory.CreateConnection())
            {
                await sqlConnection.ExecuteAsync(command, new
                {
                    UserId = user.Id,
                    LoginProvider = token.LoginProvider,
                    Name = token.Name,
                    Value = token.Value
                });
            } 
        }

        public async Task RemoveTokenAsync(TUser user, UserToken token)
        {
            var command = $"DELETE FROM [{_databaseConnectionFactory.DbSchema}].AspNetUserTokens " +
            "WHERE UserId = @UserId AND LoginProvider = @LoginProvider AND Name = @Name AND Value = @Value;";

            using (var sqlConnection = _databaseConnectionFactory.CreateConnection())
            {
                await sqlConnection.ExecuteAsync(command, new
                {
                    UserId = user.Id,
                    LoginProvider = token.LoginProvider,
                    Name = token.Name,
                    Value = token.Value
                });
            }
        }
    }
}
