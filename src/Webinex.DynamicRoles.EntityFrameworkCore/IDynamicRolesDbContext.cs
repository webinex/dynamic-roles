using Microsoft.EntityFrameworkCore;

namespace Webinex.DynamicRoles.EntityFrameworkCore
{
    /// <summary>
    ///     DbContext interface for Dynamic Roles
    /// </summary>
    /// <typeparam name="TRole">Role entity type</typeparam>
    /// <typeparam name="TRoleUser">RoleUser entity type</typeparam>
    /// <typeparam name="TRolePermission">RolePermission entity type</typeparam>
    public interface IDynamicRoleDbContext<TRole, TRoleUser, TRolePermission>
        where TRole : class
        where TRoleUser : class
        where TRolePermission : class
    {
        DbSet<TRole> Roles { get; }
        DbSet<TRoleUser> RoleUsers { get; }
        DbSet<TRolePermission> RolePermissions { get; }
    }
}