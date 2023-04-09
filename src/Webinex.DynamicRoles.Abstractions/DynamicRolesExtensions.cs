using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Webinex.DynamicRoles
{
    public static class DynamicRolesExtensions
    {
        /// <summary>
        ///     Returns user's permissions
        /// </summary>
        /// <param name="dynamicRoles"><see cref="IDynamicRoles{TRole}"/></param>
        /// <param name="userId">User identifier</param>
        /// <typeparam name="TRole">Role type</typeparam>
        /// <returns>Users permissions</returns>
        public static async Task<string[]> GetUserPermissionsAsync<TRole>(
            [NotNull] this IDynamicRoles<TRole> dynamicRoles,
            [NotNull] string userId)
        {
            dynamicRoles = dynamicRoles ?? throw new ArgumentNullException(nameof(dynamicRoles));
            userId = userId ?? throw new ArgumentNullException(nameof(userId));

            var result = await dynamicRoles.PermissionsByUserIdAsync(new[] { userId });
            return result.Values.FirstOrDefault()?.ToArray() ?? Array.Empty<string>();
        }

        /// <summary>
        ///     Returns permissions for role
        /// </summary>
        /// <param name="dynamicRoles"><see cref="IDynamicRoles{TRole}"/></param>
        /// <param name="roleId">Role identifier</param>
        /// <typeparam name="TRole">Role type</typeparam>
        /// <returns>Role's permissions</returns>
        public static async Task<string[]> GetRolePermissionsAsync<TRole>(
            [NotNull] this IDynamicRoles<TRole> dynamicRoles,
            [NotNull] string roleId)
        {
            dynamicRoles = dynamicRoles ?? throw new ArgumentNullException(nameof(dynamicRoles));
            roleId = roleId ?? throw new ArgumentNullException(nameof(roleId));

            var result = await dynamicRoles.PermissionsByRoleIdAsync(new[] { roleId });
            return result.Values.FirstOrDefault()?.ToArray() ?? Array.Empty<string>();
        }

        /// <summary>
        ///     Returns user's roles
        /// </summary>
        /// <param name="dynamicRoles"><see cref="IDynamicRoles{TRole}"/></param>
        /// <param name="userId">User identifier</param>
        /// <typeparam name="TRole">Role type</typeparam>
        /// <returns>User's roles identifiers</returns>
        public static async Task<string[]> GetUserRolesAsync<TRole>(
            [NotNull] this IDynamicRoles<TRole> dynamicRoles,
            [NotNull] string userId)
        {
            dynamicRoles = dynamicRoles ?? throw new ArgumentNullException(nameof(dynamicRoles));
            userId = userId ?? throw new ArgumentNullException(nameof(userId));

            var result = await dynamicRoles.RolesByUserIdAsync(new[] { userId });
            return result.Values.FirstOrDefault()?.ToArray() ?? Array.Empty<string>();
        }

        /// <summary>
        ///     Checks if user has any of provided permissions
        /// </summary>
        /// <param name="dynamicRoles"><see cref="IDynamicRoles{TRole}"/></param>
        /// <param name="userId">User identifier</param>
        /// <param name="permissions">Permissions to check</param>
        /// <typeparam name="TRole">Role type</typeparam>
        /// <returns>True if user has any of provided permissions, false otherwise</returns>
        public static async Task<bool> HasAnyAsync<TRole>(
            [NotNull] this IDynamicRoles<TRole> dynamicRoles,
            [NotNull] string userId,
            [NotNull] string[] permissions)
        {
            dynamicRoles = dynamicRoles ?? throw new ArgumentNullException(nameof(dynamicRoles));
            userId = userId ?? throw new ArgumentNullException(nameof(userId));
            permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));

            var userPermissions = await dynamicRoles.GetUserPermissionsAsync(userId);
            return permissions.Any(p => userPermissions.Contains(p));
        }

        /// <summary>
        ///     Checks if user has all of provided permissions
        /// </summary>
        /// <param name="dynamicRoles"><see cref="IDynamicRoles{TRole}"/></param>
        /// <param name="userId">User identifier</param>
        /// <param name="permissions">Permissions to check</param>
        /// <typeparam name="TRole">Role type</typeparam>
        /// <returns>True if user has all of provided permissions, false otherwise</returns>
        public static async Task<bool> HasAllAsync<TRole>(
            [NotNull] this IDynamicRoles<TRole> dynamicRoles,
            [NotNull] string userId,
            [NotNull] string[] permissions)
        {
            dynamicRoles = dynamicRoles ?? throw new ArgumentNullException(nameof(dynamicRoles));
            userId = userId ?? throw new ArgumentNullException(nameof(userId));
            permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));

            var userPermissions = await dynamicRoles.GetUserPermissionsAsync(userId);
            return permissions.All(p => userPermissions.Contains(p));
        }

        /// <summary>
        ///     Updates user roles
        /// </summary>
        /// <param name="dynamicRoles"><see cref="IDynamicRoles{TRole}"/></param>
        /// <param name="args">Update arguments</param>
        /// <typeparam name="TRole">Role type</typeparam>
        public static async Task UpdateUsersRolesAsync<TRole>(
            [NotNull] this IDynamicRoles<TRole> dynamicRoles,
            [NotNull] UpdateUserRolesArgs args)
        {
            dynamicRoles = dynamicRoles ?? throw new ArgumentNullException(nameof(dynamicRoles));
            args = args ?? throw new ArgumentNullException(nameof(args));

            await dynamicRoles.UpdateUsersRolesAsync(new[] { args });
        }
        
        /// <summary>
        ///     Updates role
        /// </summary>
        /// <param name="dynamicRoles"><see cref="IDynamicRoles{TRole}"/></param>
        /// <param name="args">Update arguments</param>
        /// <typeparam name="TRole">Role type</typeparam>
        public static async Task UpdateRoleAsync<TRole>(
            [NotNull] this IDynamicRoles<TRole> dynamicRoles,
            [NotNull] UpdateRoleArgs args)
        {
            dynamicRoles = dynamicRoles ?? throw new ArgumentNullException(nameof(dynamicRoles));
            args = args ?? throw new ArgumentNullException(nameof(args));

            await dynamicRoles.UpdateRolesAsync(new[] { args });
        }
    }
}