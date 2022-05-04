using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NetCore.Identity.Dapper.Factory;
using NetCore.Identity.Dapper.Interfaces;
using NetCore.Identity.Dapper.Models;
using NetCore.Identity.Dapper.Store;

namespace NetCore.Identity.Dapper.Extensions
{
    /// <summary>
    /// Extension methods on <see cref="IdentityBuilder"/> class.
    /// </summary>
    public static class IdentityBuilderExtensions
    {
        /// <summary>
        /// Adds a Dapper implementation of ASP.NET Core Identity stores.
        /// </summary>
        /// <param name="builder">Helper functions for configuring identity services.</param>
        /// <returns>The <see cref="IdentityBuilder"/> instance this method extends.</returns>
        public static IdentityBuilder AddDapperIdentityStores<TUser>(this IdentityBuilder builder, Action<DbProvider> dbProviderOptionsAction = null)
            where TUser : class, IDapperIdentityUser<TUser>
        {
            AddDefaultStores<TUser>(builder.Services, builder.UserType, builder.RoleType);
            var options = GetDefaultOptions();
            dbProviderOptionsAction?.Invoke(options);
            builder.Services.AddSingleton(options);
            builder.Services.AddScoped<IDatabaseConnectionFactory>(service => new SqlConnectionFactory(options.ConnectionString, options.DbSchema));

            return builder;
        }

        /// <summary>
        /// Adds default stores to service.
        /// </summary>
        /// <typeparam name="TUser"></typeparam>
        /// <param name="services"></param>
        /// <param name="userType"></param>
        /// <param name="roleType"></param>
        /// <exception cref="InvalidOperationException"></exception>
        private static void AddDefaultStores<TUser>(IServiceCollection services, Type userType, Type roleType) where TUser : class, IDapperIdentityUser<TUser>
        {

            if (roleType != typeof(ApplicationRole))
            {
                throw new InvalidOperationException($"{nameof(AddDapperIdentityStores)} can only be called with a role that is of type {nameof(ApplicationRole)}.");
            }
            services.AddScoped<IUserStore<TUser>, UserStore<TUser>>();
            services.AddScoped<IRoleStore<ApplicationRole>, RoleStore>();
        }

        public static DbProvider GetDefaultOptions()
        {
            return new DbProvider();
        }
    }
}
