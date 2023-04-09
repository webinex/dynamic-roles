using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Webinex.DynamicRoles
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Adds dynamic roles services
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/></param>
        /// <param name="configure">Delegate to process configuration</param>
        /// <typeparam name="TRole">Role type</typeparam>
        /// <returns><see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddDynamicRoles<TRole>(
            [NotNull] this IServiceCollection services,
            [NotNull] Action<IDynamicRolesConfiguration> configure)
        {
            services =
                services ?? throw new ArgumentNullException(nameof(services));
            configure = configure ?? throw new ArgumentNullException(nameof(configure));

            var configuration = new DynamicRolesConfiguration<TRole>(services);
            configure(configuration);
            configuration.Complete();
            
            return services;
        }
    }
}