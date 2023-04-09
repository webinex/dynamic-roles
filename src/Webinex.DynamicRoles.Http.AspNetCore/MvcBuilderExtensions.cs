using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Webinex.DynamicRoles.Http.AspNetCore
{
    public static class MvcBuilderExtensions
    {
        /// <summary>
        ///     Adds dynamic roles controller with default configuration
        /// </summary>
        /// <param name="mvcBuilder"><see cref="IMvcBuilder"/></param>
        /// <typeparam name="TRole">Role type</typeparam>
        /// <typeparam name="TRoleDto">Role DTO type</typeparam>
        /// <returns><see cref="IMvcBuilder"/></returns>
        public static IMvcBuilder AddDynamicRolesController<TRole, TRoleDto>(
            [NotNull] this IMvcBuilder mvcBuilder)
        {
            return AddDynamicRolesController<TRole, TRoleDto>(mvcBuilder, _ => { });
        }

        /// <summary>
        ///     Adds dynamic roles controller with default configuration
        /// </summary>
        /// <param name="mvcBuilder"><see cref="IMvcBuilder"/></param>
        /// <typeparam name="TRole">Role type</typeparam>
        /// <returns><see cref="IMvcBuilder"/></returns>
        public static IMvcBuilder AddDynamicRolesController<TRole>(
            [NotNull] this IMvcBuilder mvcBuilder)
        {
            return AddDynamicRolesController<TRole, TRole>(mvcBuilder, _ => { });
        }

        /// <summary>
        ///     Adds dynamic roles controller with ability to perform additional configuration
        /// </summary>
        /// <param name="mvcBuilder"><see cref="IMvcBuilder"/></param>
        /// <param name="configure">Configuration delegate</param>
        /// <typeparam name="TRole">Role type</typeparam>
        /// <returns><see cref="IMvcBuilder"/></returns>
        public static IMvcBuilder AddDynamicRolesController<TRole>(
            [NotNull] this IMvcBuilder mvcBuilder,
            [NotNull] Action<IDynamicRolesStoreControllerConfiguration> configure)
        {
            return AddDynamicRolesController<TRole, TRole>(mvcBuilder, configure);
        }

        /// <summary>
        ///     Adds dynamic roles controller with ability to perform additional configuration
        /// </summary>
        /// <param name="mvcBuilder"><see cref="IMvcBuilder"/></param>
        /// <param name="configure">Configuration delegate</param>
        /// <typeparam name="TRole">Role type</typeparam>
        /// <typeparam name="TRoleDto">Role DTO type</typeparam>
        /// <returns><see cref="IMvcBuilder"/></returns>
        public static IMvcBuilder AddDynamicRolesController<TRole, TRoleDto>(
            [NotNull] this IMvcBuilder mvcBuilder,
            [NotNull] Action<IDynamicRolesStoreControllerConfiguration> configure)
        {
            mvcBuilder = mvcBuilder ?? throw new ArgumentNullException(nameof(mvcBuilder));
            configure = configure ?? throw new ArgumentNullException(nameof(configure));

            var configuration = new DynamicRolesStoreControllerConfiguration<TRole, TRoleDto>(mvcBuilder);
            configure(configuration);

            return mvcBuilder;
        }
    }
}