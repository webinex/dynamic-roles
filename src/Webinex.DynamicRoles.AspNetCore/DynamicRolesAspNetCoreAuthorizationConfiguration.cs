using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Webinex.DynamicRoles.AspNetCore
{
    public interface IDynamicRolesAspNetCoreAuthorizationConfiguration
    {
        /// <summary>
        ///     Gives ability to use another claim as user identifier.
        ///     By default, it uses NameIdentifier claim.
        /// </summary>
        /// <param name="userIdAccessor">UserId accessor</param>
        /// <returns><see cref="IDynamicRolesAspNetCoreAuthorizationConfiguration"/></returns>
        IDynamicRolesAspNetCoreAuthorizationConfiguration UseUserIdClaimType(
            [NotNull] Func<ClaimsPrincipal, string> userIdAccessor);

        /// <summary>
        ///     Adds authorization policies by scanning provided assembly for permission attributes
        /// </summary>
        /// <param name="assemblies">Assemblies to scan</param>
        /// <returns><see cref="IDynamicRolesAspNetCoreAuthorizationConfiguration"/></returns>
        IDynamicRolesAspNetCoreAuthorizationConfiguration AddAuthorizationPoliciesFromPermissionAttributesIn(
            params Assembly[] assemblies);
    }

    internal interface IDynamicRolesAspNetCoreAuthorizationSettings
    {
        /// <summary>
        ///     User ID claim value accessor
        /// </summary>
        [NotNull]
        Func<ClaimsPrincipal, string> UserIdClaimValue { get; }
    }

    internal class DynamicRolesAspNetCoreAuthorizationConfiguration
        : IDynamicRolesAspNetCoreAuthorizationConfiguration,
            IDynamicRolesAspNetCoreAuthorizationSettings
    {
        private readonly IServiceCollection _services;
        
        public DynamicRolesAspNetCoreAuthorizationConfiguration(IServiceCollection services, Type roleType)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));

            services.AddScoped(
                typeof(IAuthorizationHandler),
                typeof(PermissionAuthorizationRequirementHandler<>).MakeGenericType(roleType));

            services.AddSingleton<IDynamicRolesAspNetCoreAuthorizationSettings>(this);
        }

        public IDynamicRolesAspNetCoreAuthorizationConfiguration UseUserIdClaimType(Func<ClaimsPrincipal, string> userIdAccessor)
        {
            UserIdClaimValue = userIdAccessor ?? throw new ArgumentNullException(nameof(userIdAccessor));
            return this;
        }

        public IDynamicRolesAspNetCoreAuthorizationConfiguration
            AddAuthorizationPoliciesFromPermissionAttributesIn(params Assembly[] assemblies)
        {
            var scanResults = PermissionAttributesScan.ScanIn(assemblies);
            _services.Configure<AuthorizationOptions>(auth =>
            {
                foreach (var result in scanResults)
                {
                    auth.AddPolicy(result.Policy, options => options
                        .AddRequirements(result.Requirement));
                }
            });

            return this;
        }

        public Func<ClaimsPrincipal, string> UserIdClaimValue { get; private set; } =
            claims => claims.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    public static class DynamicRolesAspNetCoreAuthorizationConfigurationExtensions
    {
        /// <summary>
        ///     Gives ability to use another claim as user identifier
        /// </summary>
        /// <param name="configuration"><see cref="IDynamicRolesAspNetCoreAuthorizationConfiguration"/></param>
        /// <param name="claimName">UserId claim name</param>
        /// <returns><see cref="IDynamicRolesAspNetCoreAuthorizationConfiguration"/></returns>
        public static IDynamicRolesAspNetCoreAuthorizationConfiguration UseUserIdClaimType(
            [NotNull] this IDynamicRolesAspNetCoreAuthorizationConfiguration configuration,
            [NotNull] string claimName)
        {
            configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            claimName = claimName ?? throw new ArgumentNullException(nameof(claimName));

            return configuration.UseUserIdClaimType(claims => claims.FindFirstValue(claimName));
        }
    }
}