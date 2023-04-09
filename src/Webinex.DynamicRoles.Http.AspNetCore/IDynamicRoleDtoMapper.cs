using System.Collections.Generic;
using System.Threading.Tasks;

namespace Webinex.DynamicRoles.Http.AspNetCore
{
    /// <summary>
    ///     Allows to override default Role to RoleDto mapping
    /// </summary>
    /// <typeparam name="TRole">Role type</typeparam>
    /// <typeparam name="TRoleDto">Role DTO type</typeparam>
    public interface IDynamicRoleDtoMapper<TRole, TRoleDto>
    {
        /// <summary>
        ///     Maps roles to RoleDto
        /// </summary>
        /// <param name="roles">Roles to map</param>
        /// <returns>RoleDto keyed by role identifier</returns>
        Task<IDictionary<string, TRoleDto>> MapAsync(IDictionary<string, TRole> roles);
    }
}