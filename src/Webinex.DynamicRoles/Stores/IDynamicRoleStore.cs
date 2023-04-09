using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Webinex.DynamicRoles.Stores
{
    /// <summary>
    ///     Dynamic roles data access service
    /// </summary>
    /// <typeparam name="TRole">Role type</typeparam>
    public interface IDynamicRoleStore<TRole>
    {
        /// <summary>
        ///     Creates roles
        /// </summary>
        /// <param name="args">Role creation arguments</param>
        /// <returns>Identifiers of created roles</returns>
        Task<string[]> CreateAsync([NotNull] IEnumerable<CreateRoleArgs> args);

        /// <summary>
        ///     Deletes roles by identifiers
        /// </summary>
        /// <param name="roleIds">Roles identifiers</param>
        /// <returns><see cref="Task"/></returns>
        Task DeleteAsync([NotNull] IEnumerable<string> roleIds);

        /// <summary>
        ///     Gets all roles from database
        /// </summary>
        /// <returns>Roles keyed by Role id</returns>
        Task<IDictionary<string, TRole>> RolesAsync();

        /// <summary>
        ///     Gets roles by identifiers
        /// </summary>
        /// <param name="roleIds">Role identifiers</param>
        /// <returns>Roles keyed by role id</returns>
        Task<IDictionary<string, TRole>> RolesByIdAsync([NotNull] IEnumerable<string> roleIds);

        /// <summary>
        ///     Updates roles
        /// </summary>
        /// <param name="args">Role update arguments</param>
        /// <returns><see cref="Task"/></returns>
        Task UpdateRolesAsync([NotNull] IEnumerable<UpdateRoleArgs> args);

        /// <summary>
        ///     Updates user to roles references
        /// </summary>
        /// <param name="args">Update arguments</param>
        /// <returns><see cref="Task"/></returns>
        Task UpdateUsersRolesAsync([NotNull] IEnumerable<UpdateUserRolesArgs> args);

        /// <summary>
        ///     Gets roles identifiers by user identifiers
        /// </summary>
        /// <param name="userIds">User identifiers</param>
        /// <returns>User's roles identifiers keyed by user id</returns>
        Task<IDictionary<string, IEnumerable<string>>> GetUserRolesAsync([NotNull] IEnumerable<string> userIds);

        /// <summary>
        ///     Gets permissions by user identifiers
        /// </summary>
        /// <param name="userIds">User identifiers</param>
        /// <returns>User's permissions keyed by user id</returns>
        Task<IDictionary<string, IEnumerable<string>>> GetUserPermissionsAsync([NotNull] IEnumerable<string> userIds);

        /// <summary>
        ///     Gets Role's permissions by role identifiers
        /// </summary>
        /// <param name="roleIds">Role identifiers</param>
        /// <returns>Permissions keyed by role identifier</returns>
        Task<IDictionary<string, IEnumerable<string>>> GetRolePermissionsAsync(IEnumerable<string> roleIds);

        /// <summary>
        ///     Gets users assigned to roles
        /// </summary>
        /// <param name="roleIds">Role identifiers</param>
        /// <returns>User's identifiers keyed by role identifier</returns>
        Task<IDictionary<string, IEnumerable<string>>> GetUsersByRoleIdsAsync(IEnumerable<string> roleIds);
    }
}