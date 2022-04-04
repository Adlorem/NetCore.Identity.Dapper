using NetCore.Identity.Dapper.Interfaces;
using NetCore.Identity.Dapper.Models;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NetCore.Identity.Dapper.StoreProvider
{
    internal class RoleClaimsProvider
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        public RoleClaimsProvider(IDatabaseConnectionFactory databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory;
        }

        public async Task<IList<Claim>> GetClaimsAsync(string roleId)
        {
            var command = "SELECT * " +
                                   $"FROM [{_databaseConnectionFactory.DbSchema}].[AspNetRoleClaims] " +
                                   "WHERE RoleId = @RoleId;";

            IEnumerable<UserClaim> roleClaims = new List<UserClaim>();

            using (var sqlConnection = _databaseConnectionFactory.CreateConnection())
            {
                return (await sqlConnection.QueryAsync<UserClaim>(command, new { RoleId = roleId }))
                        .Select(x => new Claim(x.ClaimType, x.ClaimValue))
                        .ToList();
            }
        }

        public async Task AddClaimAsync(ApplicationRole role, Claim claim)
        {
            var command = $"INSERT INTO [{_databaseConnectionFactory.DbSchema}].[AspNetRoleClaims] (RoleId, ClaimType, ClaimValue) " +
                                         "VALUES (RoleId, ClaimType, ClaimValue);";

            using (var sqlConnection = _databaseConnectionFactory.CreateConnection())
            {
                await sqlConnection.ExecuteAsync(command, new
                {
                    RoleId = role.Id,
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value
                });
            }
        }

        public async Task RemoveClaimAsync(ApplicationRole role, Claim claim)
        {
            var command = $"DELETE FROM [{_databaseConnectionFactory.DbSchema}].[AspNetRoleClaims] " +
            "WHERE RoleId = @RoleId AND ClaimType = @ClaimType AND ClaimValue = @ClaimValue;";

            using (var sqlConnection = _databaseConnectionFactory.CreateConnection())
            {
                await sqlConnection.ExecuteAsync(command, new
                {
                    RoleId = role.Id,
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value
                });
            }
        }
    }
}
