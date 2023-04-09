using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Webinex.DynamicRoles.EntityFrameworkCore
{
    /// <summary>
    ///     Model definition for Dynamic Role types
    /// </summary>
    /// <typeparam name="TRole">Type of Role entity</typeparam>
    /// <typeparam name="TRolePermission">Type of RolePermission entity</typeparam>
    /// <typeparam name="TRoleUser">Type of RoleUser entity</typeparam>
    public interface IDynamicRoleModelsDefinition<TRole, TRolePermission, TRoleUser>
    {
        /// <summary>
        ///     Role's identifier accessor
        /// </summary>
        Expression<Func<TRole, object>> RoleId { get; }
        
        /// <summary>
        ///     RolePermission's Role identifier accessor
        /// </summary>
        Expression<Func<TRolePermission, object>> RolePermissionRoleId { get; }
        
        /// <summary>
        ///     RolePermission's Permission kind accessor
        /// </summary>
        Expression<Func<TRolePermission, string>> RolePermissionPermission { get; }
        
        /// <summary>
        ///     RoleUser's Role identifier accessor
        /// </summary>
        Expression<Func<TRoleUser, object>> RoleUserRoleId { get; }
        
        /// <summary>
        ///     RoleUser's User identifier accessor
        /// </summary>
        Expression<Func<TRoleUser, object>> RoleUserUserId { get; }

        /// <summary>
        ///     Creates new instance of <typeparamref name="TRole"/>
        /// </summary>
        /// <param name="values">Additional values passed to CreateRoleArgs</param>
        /// <returns>New role</returns>
        TRole NewRole([NotNull] IDictionary<string, object> values);
        
        /// <summary>
        ///     Create new instance of <typeparamref name="TRolePermission"/>
        /// </summary>
        /// <param name="roleId">Role identifier</param>
        /// <param name="permission">Permission kind</param>
        /// <returns>New RolePermission</returns>
        TRolePermission NewRolePermission([NotNull] string roleId, [NotNull] string permission);
        
        /// <summary>
        ///     Creates new instance of <typeparamref name="TRoleUser"/>
        /// </summary>
        /// <param name="roleId">Role identifier</param>
        /// <param name="userId">User identifier</param>
        /// <returns>New RoleUser</returns>
        TRoleUser NewRoleUser([NotNull] string roleId, [NotNull] string userId);
        
        /// <summary>
        ///     Updates role with values passed to UpdateRoleArgs
        /// </summary>
        /// <param name="role">Role to update</param>
        /// <param name="values">Values passed to UpdateRoleArgs</param>
        void SetRoleValues([NotNull] TRole role, [NotNull] IDictionary<string, object> values);
    }
}