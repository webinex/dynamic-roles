using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Webinex.AspNetCore;

namespace Webinex.DynamicRoles.Http.AspNetCore
{
    public interface IDynamicRolesStoreControllerConfiguration
    {
        /// <summary>
        ///     Adds policy to perform authorization for all dynamic roles incoming requests.
        ///     When not specified, no authorization is performed
        /// </summary>
        /// <param name="policy">Policy name</param>
        /// <param name="schema">Authentication schema</param>
        /// <returns><see cref="IDynamicRolesStoreControllerConfiguration"/></returns>
        IDynamicRolesStoreControllerConfiguration AddPolicy([NotNull] string policy, [NotNull] string schema);
    }

    public interface IDynamicRolesStoreControllerSettings
    {
        /// <summary>
        ///     Policy to perform authorization for all dynamic roles incoming requests
        /// </summary>
        [MaybeNull]
        string Policy { get; }
        
        /// <summary>
        ///     Authentication schema
        /// </summary>
        [MaybeNull]
        string Schema { get; }
    }
    
    internal class DynamicRolesStoreControllerConfiguration<TRole, TRoleDto>
        : IDynamicRolesStoreControllerConfiguration, IDynamicRolesStoreControllerSettings
    {
        public DynamicRolesStoreControllerConfiguration(IMvcBuilder mvcBuilder)
        {
            mvcBuilder = mvcBuilder ?? throw new ArgumentNullException(nameof(mvcBuilder));
            mvcBuilder.AddController(typeof(DynamicRolesStoreController<TRole, TRoleDto>));

            mvcBuilder.Services.AddSingleton<IDynamicRolesStoreControllerSettings>(this);
            mvcBuilder.Services.TryAddSingleton<IDynamicRoleDtoMapper<TRole, TRoleDto>, DefaultDynamicRoleDtoMapper<TRole, TRoleDto>>();
        }

        public IDynamicRolesStoreControllerConfiguration AddPolicy(string policy, string schema)
        {
            Policy = policy ?? throw new ArgumentNullException(nameof(policy));
            Schema = schema ?? throw new ArgumentNullException(nameof(schema));
            return this;
        }

        public string Policy { get; private set; }
        public string Schema { get; private set; }
    }
}