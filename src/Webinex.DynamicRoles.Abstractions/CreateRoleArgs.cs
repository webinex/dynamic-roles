using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Webinex.DynamicRoles
{
    /// <summary>
    ///     Create role arguments
    /// </summary>
    public class CreateRoleArgs
    {
        public CreateRoleArgs(
            [MaybeNull] IEnumerable<string> userIds,
            [MaybeNull] IEnumerable<string> permissions,
            [MaybeNull] IDictionary<string, object> values)
        {
            UserIds = userIds?.ToArray() ?? Array.Empty<string>();
            Permissions = permissions?.ToArray() ?? Array.Empty<string>();
            Values = values ?? new Dictionary<string, object>();
        }

        /// <summary>
        ///     User identifier which might be assigned to newly created role
        /// </summary>
        [NotNull]
        public string[] UserIds { get; }

        /// <summary>
        ///     Permissions which might be assigned to newly created role
        /// </summary>
        [NotNull]
        public string[] Permissions { get; }

        /// <summary>
        ///     Role values, can be used for role model extensions
        /// </summary>
        [NotNull]
        public IDictionary<string, object> Values { get; }
    }
}