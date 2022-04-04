using NetCore.Identity.Dapper.Interfaces;
using NetCore.Identity.Dapper.Models;
using Dapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCore.Identity.Dapper.StoreProvider
{
    internal class RolesProvider
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        public RolesProvider(IDatabaseConnectionFactory databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory;
        }

        public async Task<IdentityResult> CreateAsync(ApplicationRole role)
        {
            var command = $"INSERT INTO [{_databaseConnectionFactory.DbSchema}].AspNetRoles " +
                                   "VALUES (@Id, @Name, @NormalizedName, @ConcurrencyStamp);";

            int rowsInserted;

            using (var sqlConnection = _databaseConnectionFactory.CreateConnection())
            {
                rowsInserted = await sqlConnection.ExecuteAsync(command, new {
                    role.Id,
                    role.Name,
                    role.NormalizedName,
                    role.ConcurrencyStamp
                });
            }

            return rowsInserted == 1 ? IdentityResult.Success : IdentityResult.Failed(new IdentityError {
                Code = string.Empty,
                Description = $"The role with name {role.Name} could not be inserted."
            });
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationRole role) 
        {
            var command = $"UPDATE [{_databaseConnectionFactory.DbSchema}].AspNetRoles " +
                                   "SET Name = @Name, NormalizedName = @NormalizedName, ConcurrencyStamp = @ConcurrencyStamp " +
                                   "WHERE Id = @Id;";

            using (var sqlConnection = _databaseConnectionFactory.CreateConnection())
            {
                try
                {
                    await sqlConnection.ExecuteAsync(command, new
                    {
                        role.Name,
                        role.NormalizedName,
                        role.ConcurrencyStamp,
                        role.Id

                    });
                }
                catch
                {
                    return IdentityResult.Failed(new IdentityError
                    {
                        Code = nameof(UpdateAsync),
                        Description = $"Role with name {role.Name} could not be updated."
                    });
                }
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationRole role) 
        {
            var command = "DELETE " +
                                   $"FROM [{_databaseConnectionFactory.DbSchema}].AspNetRoles " +
                                   "WHERE Id = @Id;";

            int rowsDeleted;

            using (var sqlConnection = _databaseConnectionFactory.CreateConnection()) 
            {
                rowsDeleted = await sqlConnection.ExecuteAsync(command, new { role.Id });
            }

            return rowsDeleted == 1 ? IdentityResult.Success : IdentityResult.Failed(new IdentityError {
                Code = string.Empty,
                Description = $"The role with name {role.Name} could not be deleted."
            });
        }

        public async Task<ApplicationRole> FindByIdAsync(Guid roleId) 
        {
             var command = $"SELECT * FROM [{_databaseConnectionFactory.DbSchema}].AspNetRoles " +
                                   "WHERE Id = @Id;";

            using (var sqlConnection = _databaseConnectionFactory.CreateConnection())
            {
                return await sqlConnection.QuerySingleOrDefaultAsync<ApplicationRole>(command, new
                {
                    Id = roleId
                });
            }
        }

        public async Task<ApplicationRole> FindByNameAsync(string normalizedRoleName) 
        {
             var command = $"SELECT * FROM [{_databaseConnectionFactory.DbSchema}].AspNetRoles " +
                                   "WHERE NormalizedName = @NormalizedName;";

            using (var sqlConnection = _databaseConnectionFactory.CreateConnection())
            {
                return await sqlConnection.QuerySingleOrDefaultAsync<ApplicationRole>(command, new
                {
                    NormalizedName = normalizedRoleName
                });
            }
        }

        public async Task<IEnumerable<ApplicationRole>> GetAllRolesAsync()
        {
            var command = $"SELECT * FROM [{_databaseConnectionFactory.DbSchema}].AspNetRoles;";

            using (var sqlConnection = _databaseConnectionFactory.CreateConnection())
            {
                return await sqlConnection.QueryAsync<ApplicationRole>(command);
            }
        }
    }
}
