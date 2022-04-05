using NetCore.Identity.Dapper.Interfaces;
using NetCore.Identity.Dapper.Models;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NetCore.Identity.Dapper.StoreProvider
{
    internal class UserClaimsProvider<TUser> where TUser : class, IDapperIdentityUser<TUser>
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        public UserClaimsProvider(IDatabaseConnectionFactory databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory;
        }

        public async Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            var command = $"SELECT * FROM [{_databaseConnectionFactory.DbSchema}].[AspNetUserClaims] " +
                                  "WHERE UserId = @UserId;";

            using (var sqlConnection = _databaseConnectionFactory.CreateConnection())
            {
                return (await sqlConnection.QueryAsync<UserClaim>(command, new { UserId = user.Id }))
                    .Select(e => new Claim(e.ClaimType, e.ClaimValue))
                    .ToList();
            }

        }

        public async Task AddClaimAsync(TUser user, Claim claim)
        {
            var command = $"INSERT INTO [{_databaseConnectionFactory.DbSchema}].[AspNetUserClaims] (UserId, ClaimType, ClaimValue) " +
                             "VALUES (@UserId, @ClaimType, @ClaimValue);";

            using (var sqlConnection = _databaseConnectionFactory.CreateConnection())
            {
                await sqlConnection.ExecuteAsync(command, new
                {
                    UserId = user.Id,
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value
                });
            }
        }

        public async Task RemoveClaimAsync(TUser user, Claim claim)
        {
            var command = $"DELETE FROM [{_databaseConnectionFactory.DbSchema}].[AspNetUserClaims] " +
                                         "WHERE UserId = @UserId AND ClaimType = @ClaimType AND ClaimValue = @ClaimValue;";

            using (var sqlConnection = _databaseConnectionFactory.CreateConnection())
            {
                await sqlConnection.ExecuteAsync(command, new
                {
                    UserId = user.Id,
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value
                });
            }
        }
    }
}
