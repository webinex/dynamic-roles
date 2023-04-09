using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Webinex.DynamicRoles
{
    /// <summary>
    ///     Updates role
    /// </summary>
    public class UpdateRoleArgs
    {
        /// <summary>
        ///     Creates new instance of <see cref="UpdateRoleArgs"/>
        /// </summary>
        /// <param name="roleId">Role identifier</param>
        /// <param name="userIds">
        ///     Role users might be updated to match provided user ids.
        ///     If null - users will not be updated.
        ///     If empty - all user's references would be deleted.
        /// </param>
        /// <param name="permissions">
        ///     Role permissions might be updated to match provided permissions.
        ///     If null - permissions will not be updated.
        ///     If empty - all permissions references would be deleted.
        /// </param>
        /// <param name="values">
        ///     Additional values. Can be used to update extended role values like a name or description.
        /// </param>
        /// <exception cref="ArgumentNullException"></exception>
        public UpdateRoleArgs(
            [NotNull] string roleId,
            [MaybeNull] IEnumerable<string> userIds,
            [MaybeNull] IEnumerable<string> permissions,
            [MaybeNull] IDictionary<string, object> values)
        {
            Id = roleId ?? throw new ArgumentNullException(nameof(roleId));
            Values = values;
            UserIds = userIds?.ToArray();
            Permissions = permissions?.ToArray();
        }

        /// <summary>
        ///     Role identifier
        /// </summary>
        [NotNull]
        public string Id { get; }
        
        /// <summary>
        ///     Additional values. Can be used to update extended role values like a name or description.
        /// </summary>
        [MaybeNull]
        public IDictionary<string, object> Values { get; }

        /// <summary>
        ///     Role users might be updated to match provided user ids.
        ///     If null - users will not be updated.
        ///     If empty - all user's references would be deleted.
        /// </summary>
        [MaybeNull]
        public string[] UserIds { get; }
        
        /// <summary>
        ///     Role permissions might be updated to match provided permissions.
        ///     If null - permissions will not be updated.
        ///     If empty - all permissions references would be deleted.
        /// </summary>
        public string[] Permissions { get; }

        /// <summary>
        ///     When false - Permissions value might be ignored.
        /// </summary>
        public bool ShouldUpdatePermissions => Permissions != null;

        /// <summary>
        ///     When false - UserIds value might be ignored.
        /// </summary>
        public bool ShouldUpdateUsers => UserIds != null;

        /// <summary>
        ///     When false - Values value might be ignored.
        /// </summary>
        public bool ShouldUpdateModel => Values != null;
    }
}