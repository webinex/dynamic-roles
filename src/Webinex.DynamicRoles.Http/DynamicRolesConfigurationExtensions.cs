using System;
using System.Diagnostics.CodeAnalysis;

namespace Webinex.DynamicRoles.Http
{
    public static class DynamicRolesConfigurationExtensions
    {
        /// <summary>
        ///     Adds http based stores to DynamicRoles
        /// </summary>
        /// <param name="dynamicRolesConfiguration"><see cref="IDynamicRolesConfiguration"/></param>
        /// <returns><see cref="IDynamicRolesConfiguration"/></returns>
        public static IDynamicRolesConfiguration AddHttp(
            [NotNull] this IDynamicRolesConfiguration dynamicRolesConfiguration)
        {
            dynamicRolesConfiguration = dynamicRolesConfiguration ?? throw new ArgumentNullException(nameof(dynamicRolesConfiguration));
            return AddHttp(dynamicRolesConfiguration, _ => { });
        }

        /// <summary>
        ///     Adds http based stores to DynamicRoles
        /// </summary>
        /// <param name="dynamicRolesConfiguration"><see cref="IDynamicRolesConfiguration"/></param>
        /// <param name="configure">Delegate to process additional configuration</param>
        /// <returns><see cref="IDynamicRolesConfiguration"/></returns>
        public static IDynamicRolesConfiguration AddHttp(
            [NotNull] this IDynamicRolesConfiguration dynamicRolesConfiguration,
            [NotNull] Action<IDynamicRolesHttpConfiguration> configure)
        {
            dynamicRolesConfiguration = dynamicRolesConfiguration ?? throw new ArgumentNullException(nameof(dynamicRolesConfiguration));
            configure = configure ?? throw new ArgumentNullException(nameof(configure));

            var configuration =
                new DynamicRolesHttpConfiguration(dynamicRolesConfiguration.Services, dynamicRolesConfiguration.RoleType);
            configure(configuration);

            return dynamicRolesConfiguration;
        }
    }
}