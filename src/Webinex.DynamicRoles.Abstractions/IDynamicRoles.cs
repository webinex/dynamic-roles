using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Webinex.DynamicRoles
{
    // TODO: s.skalaban, add UsersByRoleId
    public interface IDynamicRoles<TRole>
    {
        /// <summary>
        ///     Returns permissions configuration
        /// </summary>
        /// <returns>Returns permissions configuration</returns>
        Task<PermissionsConfiguration> PermissionsConfigurationAsync();

        /// <summary>
        ///     Creates new roles
        /// </summary>
        /// <param name="args">Role creating arguments</param>
        /// <returns>Identifier of newly created roless</returns>
        Task<string[]> CreateRolesAsync([NotNull] IEnumerable<CreateRoleArgs> args);

        /// <summary>
        ///     Updates roles
        /// </summary>
        /// <param name="args">Update arguments</param>
        /// <returns><see cref="Task"/></returns>
        Task UpdateRolesAsync([NotNull] IEnumerable<UpdateRoleArgs> args);

        /// <summary>
        ///     Updates user roles
        /// </summary>
        /// <param name="args">User roles arguments</param>
        /// <returns><see cref="Task"/></returns>
        Task UpdateUsersRolesAsync([NotNull] IEnumerable<UpdateUserRolesArgs> args);

        /// <summary>
        ///     Returns user role identifiers keyed by user id
        /// </summary>
        /// <param name="userIds">User identifier</param>
        /// <returns>User role identifiers keyed by user id</returns>
        Task<IDictionary<string, IEnumerable<string>>> RolesByUserIdAsync([NotNull] IEnumerable<string> userIds);

        /// <summary>
        ///     Returns permissions keyed by user identifier
        /// </summary>
        /// <param name="userIds">User identifiers</param>
        /// <returns>Permissions by user id</returns>
        Task<IDictionary<string, IEnumerable<string>>> PermissionsByUserIdAsync([NotNull] IEnumerable<string> userIds);

        /// <summary>
        ///     Returns permissions keyed by role identifier
        /// </summary>
        /// <param name="roleIds">Role identifiers</param>
        /// <returns>Permissions by role id</returns>
        Task<IDictionary<string, IEnumerable<string>>> PermissionsByRoleIdAsync([NotNull] IEnumerable<string> roleIds);

        /// <summary>
        ///     Returns all roles as dictionary keyed by role identifier
        /// </summary>
        /// <returns>Roles keyed by role identifier</returns>
        Task<IDictionary<string, TRole>> RolesAsync();

        /// <summary>
        ///     Deletes roles from storage
        /// </summary>
        /// <param name="roleIds">Identifiers of roles to delete</param>
        /// <returns><see cref="Task"/></returns>
        Task DeleteRolesAsync([NotNull] IEnumerable<string> roleIds);

        /// <summary>
        ///     Returns roles with ids matching <paramref name="roleIds"/> keyed by role identifiers
        /// </summary>
        /// <param name="roleIds">Role identifiers</param>
        /// <returns>Roles keyed by role identifier</returns>
        Task<IDictionary<string, TRole>> RolesByIdAsync([NotNull] IEnumerable<string> roleIds);

        /// <summary>
        ///     Returns users assigned to roles
        /// </summary>
        /// <param name="roleIds">Role identifiers</param>
        /// <returns>User identifiers keyed by role identifiers</returns>
        Task<IDictionary<string, IEnumerable<string>>> UsersByRoleIdAsync([NotNull] IEnumerable<string> roleIds);
    }
}