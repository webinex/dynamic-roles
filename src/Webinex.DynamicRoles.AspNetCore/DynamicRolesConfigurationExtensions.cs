using System;
using System.Diagnostics.CodeAnalysis;

namespace Webinex.DynamicRoles.AspNetCore
{
    public static class DynamicRolesConfigurationExtensions
    {
        /// <summary>
        ///     Adds dynamic roles asp net authorization services
        /// </summary>
        /// <param name="dynamicRolesConfiguration"><see cref="IDynamicRolesConfiguration"/></param>
        /// <returns><see cref="IDynamicRolesConfiguration"/></returns>
        public static IDynamicRolesConfiguration AddAspNetAuthorization(
            [NotNull] this IDynamicRolesConfiguration dynamicRolesConfiguration)
        {
            dynamicRolesConfiguration = dynamicRolesConfiguration
                                        ?? throw new ArgumentNullException(nameof(dynamicRolesConfiguration));

            return AddAspNetAuthorization(dynamicRolesConfiguration, _ => { });
        }

        /// <summary>
        ///     Adds dynamic roles asp net authorization services with additional configuration
        /// </summary>
        /// <param name="dynamicRolesConfiguration"><see cref="IDynamicRolesConfiguration"/></param>
        /// <param name="configure">Gives ability to make additional configuration</param>
        /// <returns><see cref="IDynamicRolesConfiguration"/></returns>
        public static IDynamicRolesConfiguration AddAspNetAuthorization(
            [NotNull] this IDynamicRolesConfiguration dynamicRolesConfiguration,
            [NotNull] Action<IDynamicRolesAspNetCoreAuthorizationConfiguration> configure)
        {
            dynamicRolesConfiguration = dynamicRolesConfiguration
                                        ?? throw new ArgumentNullException(nameof(dynamicRolesConfiguration));
            configure = configure ?? throw new ArgumentNullException(nameof(configure));

            var configuration = new DynamicRolesAspNetCoreAuthorizationConfiguration(
                dynamicRolesConfiguration.Services,
                dynamicRolesConfiguration.RoleType);

            configure(configuration);

            return dynamicRolesConfiguration;
        }
    }
}