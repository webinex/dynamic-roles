using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Webinex.DynamicRoles
{
    /// <summary>
    ///     Permissions configuration
    /// </summary>
    public class PermissionsConfiguration
    {
        private readonly IDictionary<string, PermissionConfiguration> _byKind;

        /// <summary>
        ///     Creates new instance of permission configurations
        /// </summary>
        /// <param name="permissions">Known permissions</param>
        public PermissionsConfiguration([NotNull] IEnumerable<PermissionConfiguration> permissions)
        {
            var array = permissions?.ToArray() ?? throw new ArgumentNullException(nameof(permissions));
            
            if (array.Any(x => x == null))
                throw new ArgumentException("Might not contain nulls", nameof(permissions));

            Permissions = array;
            _byKind = Permissions.ToDictionary(x => x.Kind);
        }

        /// <summary>
        ///     Known permissions configurations
        /// </summary>
        public PermissionConfiguration[] Permissions { get; }

        /// <summary>
        ///     Checks is permission configuration with <paramref name="kind"/> exists
        /// </summary>
        /// <param name="kind">Kind of permission to check</param>
        /// <returns>True if exists, false otherwise</returns>
        public bool Has(string kind)
        {
            kind = kind ?? throw new ArgumentNullException(nameof(kind));
            return _byKind.ContainsKey(kind);
        }

        /// <summary>
        ///     Returns permission configuration by kind
        /// </summary>
        /// <param name="kind">Kind of requested permission</param>
        /// <returns>Permission configuration</returns>
        public PermissionConfiguration ByKind([NotNull] string kind)
        {
            kind = kind ?? throw new ArgumentNullException(nameof(kind));
            return _byKind[kind];
        }
    }
}