using System;
using Microsoft.Extensions.DependencyInjection;
using Webinex.DynamicRoles.Stores;

namespace Webinex.DynamicRoles.Http
{
    public interface IDynamicRolesHttpConfiguration
    {
        /// <summary>
        ///     Adds http factory for Http roles store
        /// </summary>
        /// <param name="lifetime">Lifetime to register</param>
        /// <typeparam name="T">Type of http factory</typeparam>
        /// <returns><see cref="IDynamicRolesHttpConfiguration"/></returns>
        IDynamicRolesHttpConfiguration AddHttpFactory<T>(ServiceLifetime lifetime = ServiceLifetime.Transient)
            where T : class, IDynamicRolesHttpClientFactory;
    }

    internal class DynamicRolesHttpConfiguration : IDynamicRolesHttpConfiguration
    {
        private readonly IServiceCollection _services;

        public DynamicRolesHttpConfiguration(IServiceCollection services, Type roleType)
        {
            _services = services ?? throw new ArgumentNullException();
            roleType = roleType ?? throw new ArgumentNullException(nameof(roleType));

            _services.AddScoped(
                typeof(IDynamicRoleStore<>).MakeGenericType(roleType),
                typeof(DynamicRolesHttpClientStore<>).MakeGenericType(roleType));

            _services.AddScoped(
                typeof(IPermissionsConfigurationStore),
                typeof(DynamicRolesHttpClientStore<>).MakeGenericType(roleType));
        }

        public IDynamicRolesHttpConfiguration AddHttpFactory<T>(
            ServiceLifetime lifetime = ServiceLifetime.Transient)
            where T : class, IDynamicRolesHttpClientFactory
        {
            _services.Add(new ServiceDescriptor(typeof(IDynamicRolesHttpClientFactory), typeof(T), lifetime));
            
            return this;
        }
    }
}