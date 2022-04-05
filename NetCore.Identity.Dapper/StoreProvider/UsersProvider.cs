using NetCore.Identity.Dapper.Extensions;
using NetCore.Identity.Dapper.Interfaces;
using Dapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NetCore.Identity.Dapper.StoreProvider
{
    internal class UsersProvider<TUser> where TUser : class, IDapperIdentityUser<TUser>
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        public UsersProvider(IDatabaseConnectionFactory databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory;
        }

        public async Task<IdentityResult> CreateAsync(TUser user)
        {
            // In this scenario we need to generate query dynamically as additonal parameters can be added
            // to application user model.
            var command = user.DapperCreateInsert("AspNetUsers");
            var properties = user.DapperCreateProperties();

            int rowsInserted;

            using (var sqlConnection = _databaseConnectionFactory.CreateConnection())
            {
                rowsInserted = await sqlConnection.ExecuteAsync(command, properties);
            }

            return rowsInserted == 1 ? IdentityResult.Success : IdentityResult.Failed(new IdentityError
            {
                Code = nameof(CreateAsync),
                Description = $"User with email {user.Email} could not be inserted."
            });
        }

        public async Task<IdentityResult> DeleteAsync(TUser user) {
            var command = $"DELETE FROM [{_databaseConnectionFactory.DbSchema}].[AspNetUsers] " +
                                   "WHERE Id = @Id;";

            int rowsDeleted;

            using (var sqlConnection = _databaseConnectionFactory.CreateConnection())
            {
                rowsDeleted = await sqlConnection.ExecuteAsync(command, new
                {
                    user.Id
                });
            }

            return rowsDeleted == 1 ? IdentityResult.Success : IdentityResult.Failed(new IdentityError {
                Code = nameof(DeleteAsync),
                Description = $"User with email {user.Email} could not be deleted."
            });
        }

        public async Task<TUser> FindByIdAsync(Guid userId)
        {
            var command = $"SELECT * FROM [{_databaseConnectionFactory.DbSchema}].[AspNetUsers] " +
                                   "WHERE Id = @Id;";

            using (var sqlConnection = _databaseConnectionFactory.CreateConnection())
            {
                return await sqlConnection.QuerySingleOrDefaultAsync<TUser>(command, new
                {
                    Id = userId
                });
            }
        }

        public async Task<TUser> FindByNameAsync(string normalizedUserName)
        {
            var command = $"SELECT * FROM [{_databaseConnectionFactory.DbSchema}].[AspNetUsers] " +
                                   "WHERE NormalizedUserName = @NormalizedUserName;";

            using (var sqlConnection = _databaseConnectionFactory.CreateConnection())
            {
                return await sqlConnection.QuerySingleOrDefaultAsync<TUser>(command, new
                {
                    NormalizedUserName = normalizedUserName
                });
            }
        }

        public async Task<TUser> FindByEmailAsync(string normalizedEmail) 
        {
            var command = $"SELECT * FROM [{_databaseConnectionFactory.DbSchema}].[AspNetUsers] " +
                                   "WHERE NormalizedEmail = @NormalizedEmail;";

            using (var sqlConnection = _databaseConnectionFactory.CreateConnection())
            {
                return await sqlConnection.QuerySingleOrDefaultAsync<TUser>(command, new
                {
                    NormalizedEmail = normalizedEmail
                });
            }
        }

        public async Task<IdentityResult> UpdateAsync(TUser user) 
        {
            var command = user.DapperCreateUpdate("AspNetUsers", "Id");
            var parameters = user.DapperCreateProperties();

            using (var sqlConnection = _databaseConnectionFactory.CreateConnection())
            {
                try
                {
                    await sqlConnection.ExecuteAsync(command, parameters);
                }
                catch
                {
                    return IdentityResult.Failed(new IdentityError
                    {
                        Code = nameof(UpdateAsync),
                        Description = $"User with email {user.Email} could not be updated."
                    });
                }
            }

            return IdentityResult.Success;
        }

        public async Task<IList<TUser>> GetUsersInRoleAsync(string roleName)
        {
            var command = $"SELECT * FROM [{_databaseConnectionFactory.DbSchema}].[AspNetUsers] AS u " +
                            $"INNER JOIN [{_databaseConnectionFactory.DbSchema}].[AspNetUserRoles] AS ur ON u.Id = ur.UserId " +
                            $"INNER JOIN [{_databaseConnectionFactory.DbSchema}].AspNetRoles AS r ON ur.RoleId = r.Id " +
                            "WHERE r.Name = @RoleName;";

            using (var sqlConnection = _databaseConnectionFactory.CreateConnection())
            {
                return (await sqlConnection.QueryAsync<TUser>(command, new
                {
                    RoleName = roleName
                })).ToList();
            }
        }

        public async Task<IList<TUser>> GetUsersForClaimAsync(Claim claim)
        {
            var command = $"SELECT * FROM [{_databaseConnectionFactory.DbSchema}].[AspNetUsers] AS u " +
                                   $"INNER JOIN [{_databaseConnectionFactory.DbSchema}].[AspNetUserClaims] AS uc ON u.Id = uc.UserId " +
                                   "WHERE uc.ClaimType = @ClaimType AND uc.ClaimValue = @ClaimValue;";

            using (var sqlConnection = _databaseConnectionFactory.CreateConnection())
            {
                return (await sqlConnection.QueryAsync<TUser>(command, new
                {
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value
                })).ToList();
            }
        }

        public async Task<IEnumerable<TUser>> GetAllUsers()
        {
            var command = $"SELECT * FROM [{_databaseConnectionFactory.DbSchema}].[AspNetUsers];";

            using (var sqlConnection = _databaseConnectionFactory.CreateConnection())
            {
                return await sqlConnection.QueryAsync<TUser>(command);
            }
        }
    }
}
