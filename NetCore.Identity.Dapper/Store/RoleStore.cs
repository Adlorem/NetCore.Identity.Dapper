using NetCore.Identity.Dapper.Interfaces;
using NetCore.Identity.Dapper.Models;
using NetCore.Identity.Dapper.StoreProvider;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace NetCore.Identity.Dapper.Store
{
    /// <summary>
    /// Dapper based implementation.
    /// </summary>
    public class RoleStore : IQueryableRoleStore<ApplicationRole>, IRoleClaimStore<ApplicationRole>, IRoleStore<ApplicationRole>
    {
        private RolesProvider _roleStoreProvider;
        private RoleClaimsProvider _claimStoreProvider;

        public RoleStore(IDatabaseConnectionFactory connection)
        {
            _roleStoreProvider = new RolesProvider(connection);
            _claimStoreProvider = new RoleClaimsProvider(connection);
        }

        public IQueryable<ApplicationRole> Roles => Task.Run(() => _roleStoreProvider.GetAllRolesAsync()).Result.AsQueryable();

        public async Task<IdentityResult> CreateAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            return await _roleStoreProvider.CreateAsync(role);
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            return await _roleStoreProvider.UpdateAsync(role);
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            return await _roleStoreProvider.DeleteAsync(role);
        }

        public Task<string> GetRoleIdAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            return Task.FromResult(role.Id);
        }

        public Task<string> GetRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            return Task.FromResult(role.Name);
        }

        public Task SetRoleNameAsync(ApplicationRole role, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException(nameof(roleName));
            }
            role.Name = roleName;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            return Task.FromResult(role.NormalizedName);
        }

        public Task SetNormalizedRoleNameAsync(ApplicationRole role, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            if (string.IsNullOrEmpty(normalizedName))
            {
                throw new ArgumentNullException(nameof(normalizedName));
            }
            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public async Task<ApplicationRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (string.IsNullOrEmpty(roleId))
            {
                throw new ArgumentNullException(nameof(roleId));
            }
            var isValidGuid = Guid.TryParse(roleId, out var roleGuid);

            if (!isValidGuid)
            {
                throw new ArgumentException("Parameter roleId is not a valid Guid.", nameof(roleId));
            }

            return await _roleStoreProvider.FindByIdAsync(roleGuid);
        }

        public async Task<ApplicationRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (string.IsNullOrEmpty(normalizedRoleName))
            {
                throw new ArgumentNullException(nameof(normalizedRoleName));
            }
            return await _roleStoreProvider.FindByNameAsync(normalizedRoleName);
        }


        public async Task<IList<Claim>> GetClaimsAsync(ApplicationRole role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            return (await _claimStoreProvider.GetClaimsAsync(role.Id)).ToList();
        }

        public async Task AddClaimAsync(ApplicationRole role, Claim claim, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            var roleClaims = (await _claimStoreProvider.GetClaimsAsync(role.Id)).ToList();
            var foundClaim = roleClaims.FirstOrDefault(x => x.Type == claim.Type);

            if (foundClaim != null)
            {
                await _claimStoreProvider.RemoveClaimAsync(role, claim);
                await _claimStoreProvider.AddClaimAsync(role, claim);
            }
            else
            {
                await _claimStoreProvider.AddClaimAsync(role, claim);
            }
        }

        public async Task RemoveClaimAsync(ApplicationRole role, Claim claim, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            await _claimStoreProvider.RemoveClaimAsync(role, claim);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~RoleStore()
        {
            Dispose(false);
        }
    }
}