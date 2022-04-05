using NetCore.Identity.Dapper.Interfaces;
using NetCore.Identity.Dapper.Models;
using Dapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore.Identity.Dapper.StoreProvider
{
    internal class UserLoginsProvider<TUser> where TUser : class, IDapperIdentityUser<TUser>
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        public UserLoginsProvider(IDatabaseConnectionFactory databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory;
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user) {
            var command = $"SELECT * FROM [{_databaseConnectionFactory.DbSchema}].[AspNetUserLogins] " +
                                   "WHERE UserId = @UserId;";

            using (var sqlConnection = _databaseConnectionFactory.CreateConnection())
            {
                return (await sqlConnection.QueryAsync<UserLogin>(command, new { UserId = user.Id }))
                            .Select(x => new UserLoginInfo(x.LoginProvider, x.ProviderKey, x.ProviderDisplayName))
                            .ToList();
            }

        }

        public async Task<TUser> FindByLoginAsync(string loginProvider, string providerKey) 
        {
            var command = $"SELECT UserId FROM [{_databaseConnectionFactory.DbSchema}].[AspNetUserLogins] " +
                "WHERE LoginProvider = @LoginProvider AND ProviderKey = @ProviderKey;";


            using (var sqlConnection = _databaseConnectionFactory.CreateConnection())
            {
                var userId = await sqlConnection.QuerySingleOrDefaultAsync<Guid?>(command, new
                {
                    LoginProvider = loginProvider,
                    ProviderKey = providerKey
                });

                if (userId == null)
                {
                    return null;
                }

                command = $"SELECT * FROM [{_databaseConnectionFactory.DbSchema}].[AspNetUsers] " +
                          "WHERE Id = @Id;";

                return await sqlConnection.QuerySingleAsync<TUser>(command, new { Id = userId });
            }
        }

        public async Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            var command = $"INSERT INTO [{_databaseConnectionFactory.DbSchema}].[AspNetUserLogins] (UserId, LoginProvider, ProviderKey, ProviderDisplayName) " +
                                         "VALUES (@UserId@, LoginProvider, @ProviderKey, @ProviderDisplayName);";

            using (var sqlConnection = _databaseConnectionFactory.CreateConnection())
            {
                await sqlConnection.ExecuteAsync(command, new
                {
                    UserId = user.Id,
                    LoginProvider = login.LoginProvider,
                    ProviderKey = login.ProviderKey,
                    ProviderDisplayName = login.ProviderDisplayName,
                });
            }
        }

        public async Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            var command = $"DELETE FROM [{_databaseConnectionFactory.DbSchema}].[AspNetUserLogins] " +
            "WHERE UserId = @UserId AND LoginProvider = @LoginProvider AND ProviderKey = @ProviderKey AND ProviderDisplayName = @ProviderDisplayName;";

            using (var sqlConnection = _databaseConnectionFactory.CreateConnection())
            {
                await sqlConnection.ExecuteAsync(command, new
                {
                    UserId = user.Id,
                    LoginProvider = login.LoginProvider,
                    ProviderKey = login.ProviderKey,
                    ProviderDisplayName = login.ProviderDisplayName,
                });
            }
        }
    }
}
