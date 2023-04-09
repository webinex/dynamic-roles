using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Webinex.DynamicRoles.Stores;
using Webinex.DynamicRoles.UserPermissionsCaches;

namespace Webinex.DynamicRoles
{
    public interface IDynamicRolesConfiguration
    {
        /// <summary>
        ///     Dynamic role type
        /// </summary>
        [NotNull]
        Type RoleType { get; }

        /// <summary>
        ///     Service collection. Useful in child packages
        /// </summary>
        [NotNull]
        IServiceCollection Services { get; }
        
        /// <summary>
        ///     Configuration values. Useful to share data between calls in child packages
        /// </summary>
        [NotNull]
        IDictionary<string, object> Values { get; }

        /// <summary>
        ///     Adds permission configuration in memory store
        /// </summary>
        /// <param name="configuration">Permissions configuration</param>
        /// <returns><see cref="IDynamicRolesConfiguration"/></returns>
        IDynamicRolesConfiguration AddInMemoryPermissionsConfiguration([NotNull] PermissionsConfiguration configuration);

        /// <summary>
        ///     Adds in memory permissions cache for user permissions
        /// </summary>
        /// <param name="prefix">Cache key prefix</param>
        /// <param name="expiration">Cache entry time to live</param>
        /// <returns><see cref="IDynamicRolesConfiguration"/></returns>
        IDynamicRolesConfiguration AddUsersPermissionsMemoryCache(string prefix = null, TimeSpan? expiration = null);
    }

    internal interface IDynamicRolesSettings
    {
    }

    internal class DynamicRolesConfiguration<TRole> : IDynamicRolesConfiguration,
        IDynamicRolesSettings,
        IUserPermissionsMemoryCacheSettings
    {
        public DynamicRolesConfiguration(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));

            services
                .AddSingleton<IDynamicRolesSettings>(this)
                .AddScoped<IDynamicRoles<TRole>, DynamicRoles<TRole>>()
                .AddScoped<IPermissionsValidator, PermissionsValidator>()
                .AddScoped<IPermissionConfigurationService, PermissionConfigurationService>()
                .AddSingleton<IUserPermissionsMemoryCacheSettings>(this);
        }

        public Type RoleType => typeof(TRole);

        public IServiceCollection Services { get; }

        public IDictionary<string, object> Values { get; } = new Dictionary<string, object>();

        public TimeSpan Expiration { get; private set; } = TimeSpan.FromMinutes(15);

        public string Prefix { get; private set; } = "dynamic-roles-users-permissions";

        public IDynamicRolesConfiguration AddInMemoryPermissionsConfiguration(PermissionsConfiguration configuration)
        {
            configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Services.AddSingleton<IPermissionsConfigurationStore>(
                new InMemoryPermissionConfigurationStore(configuration));

            return this;
        }

        public IDynamicRolesConfiguration AddUsersPermissionsMemoryCache(string prefix = null, TimeSpan? expiration = null)
        {
            if (prefix != null) Prefix = prefix;
            if (expiration != null) Expiration = expiration.Value;

            Services.AddScoped<IUserPermissionsCache, MemoryUserPermissionsCache>();
            return this;
        }

        public void Complete()
        {
            Services.TryAddSingleton<IUserPermissionsCache, NullUserPermissionsCache>();
        }
    }
}