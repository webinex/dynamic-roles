using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;

namespace Webinex.DynamicRoles.AspNetCore
{
    public static class PermissionAttributesScan
    {
        /// <summary>
        ///     Scans assemblies for permission attributes
        /// </summary>
        /// <param name="assemblies">Assemblies to scan</param>
        /// <returns>Objects which contains lexical policy names and permission authorization requirements</returns>
        public static Result[] ScanIn(params Assembly[] assemblies)
        {
            return assemblies.SelectMany(ScanInAssembly).ToArray();
        }

        private static Result[] ScanInAssembly(Assembly assembly)
        {
            return assembly
                .GetTypes()
                .SelectMany(x => x.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Select(m => m.GetCustomAttribute<PermissionAttributeBase>())
                    .Where(attr => attr != null))
                .Select(Create)
                .ToArray();
        }

        private static Result Create(PermissionAttributeBase attr)
        {
            var requirement = new PermissionAuthorizationRequirement(attr.Expression.Predicate);
            return new Result(attr.Policy, requirement);
        }

        public class Result
        {
            public Result(string policy, IAuthorizationRequirement requirement)
            {
                Policy = policy ?? throw new ArgumentNullException(nameof(policy));
                Requirement = requirement ?? throw new ArgumentNullException(nameof(requirement));
            }

            public string Policy { get; }
            public IAuthorizationRequirement Requirement { get; }
        }
    }
}