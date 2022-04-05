using NetCore.Identity.Dapper;
using NetCore.Identity.Dapper.Interfaces;
using NetCore.Identity.Dapper.Models;
using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCore.Identity.Dapper.StoreProvider
{
    internal class UserRolesProvider<TUser> where TUser : class, IDapperIdentityUser<TUser>
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        public UserRolesProvider(IDatabaseConnectionFactory databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory;
        }

        public async Task<IEnumerable<ApplicationRole>> GetRolesAsync(TUser user)
        {
            var command = "SELECT r.Id AS RoleId, r.Name AS RoleName " +
                                   $"FROM [{_databaseConnectionFactory.DbSchema}].AspNetRoles AS r " +
                                   $"INNER JOIN [{_databaseConnectionFactory.DbSchema}].[AspNetUserRoles] AS ur ON ur.RoleId = r.Id " +
                                   "WHERE ur.UserId = @UserId;";

            using (var sqlConnection = _databaseConnectionFactory.CreateConnection())
            {
                return await sqlConnection.QueryAsync<ApplicationRole>(command, new
                {
                    UserId = user.Id
                });
            }
        }

        public async Task AddToRoleAsync(TUser user, ApplicationRole role)
        {
            var command = $"INSERT INTO [{_databaseConnectionFactory.DbSchema}].[AspNetUserRoles] (UserId, RoleId) " +
                            "VALUES (@UserId, @RoleId);";

            using (var sqlConnection = _databaseConnectionFactory.CreateConnection())
            {
                await sqlConnection.ExecuteAsync(command, new
                {
                    UserId = user.Id,
                    RoleId = role.Id
                });
            }
        }

        public async Task RemoveFromRoleAsync(TUser user, ApplicationRole role)
        {
            var command = $"DELETE FROM [{_databaseConnectionFactory.DbSchema}].[AspNetUserRoles] " +
                                        "WHERE UserId = @UserId AND RoleId = @RoleId;";

            using (var sqlConnection = _databaseConnectionFactory.CreateConnection())
            {
                await sqlConnection.ExecuteAsync(command, new
                {
                    UserId = user.Id,
                    RoleId = role.Id
                });
            }
        }
    }
}
