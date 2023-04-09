using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Webinex.DynamicRoles
{
    /// <summary>
    ///     Update user roles arguments
    /// </summary>
    public class UpdateUserRolesArgs
    {
        /// <summary>
        ///     Creates new instance of <see cref="UpdateUserRolesArgs"/>
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <param name="roleIds">Target role ids</param>
        public UpdateUserRolesArgs([NotNull] string userId, [NotNull] IEnumerable<string> roleIds)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            RoleIds = roleIds?.ToArray() ?? throw new ArgumentNullException(nameof(roleIds));
        }

        /// <summary>
        ///     User identifier
        /// </summary>
        [NotNull]
        public string UserId { get; }
        
        /// <summary>
        ///     Target user roles state
        /// </summary>
        [NotNull]
        public string[] RoleIds { get; }
    }
}