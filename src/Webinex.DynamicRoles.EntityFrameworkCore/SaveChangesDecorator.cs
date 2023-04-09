using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Webinex.DynamicRoles.EntityFrameworkCore
{
    internal class SaveChangesDecorator<TRole, TDbContext> : IDynamicRoles<TRole>
        where TDbContext : DbContext
    {
        private readonly IDynamicRoles<TRole> _next;
        private readonly TDbContext _dbContext;

        public SaveChangesDecorator(IDynamicRoles<TRole> next, TDbContext dbContext)
        {
            _next = next;
            _dbContext = dbContext;
        }

        public Task<PermissionsConfiguration> PermissionsConfigurationAsync()
        {
            return _next.PermissionsConfigurationAsync();
        }

        public async Task<string[]> CreateRolesAsync(IEnumerable<CreateRoleArgs> args)
        {
            var result = await _next.CreateRolesAsync(args);
            await _dbContext.SaveChangesAsync();
            return result;
        }

        public async Task UpdateRolesAsync(IEnumerable<UpdateRoleArgs> args)
        {
            await _next.UpdateRolesAsync(args);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateUsersRolesAsync(IEnumerable<UpdateUserRolesArgs> args)
        {
            await _next.UpdateUsersRolesAsync(args);
            await _dbContext.SaveChangesAsync();
        }

        public Task<IDictionary<string, IEnumerable<string>>> RolesByUserIdAsync(IEnumerable<string> userIds)
        {
            return _next.RolesByUserIdAsync(userIds);
        }

        public Task<IDictionary<string, IEnumerable<string>>> PermissionsByUserIdAsync(IEnumerable<string> userIds)
        {
            return _next.PermissionsByUserIdAsync(userIds);
        }

        public Task<IDictionary<string, IEnumerable<string>>> PermissionsByRoleIdAsync(IEnumerable<string> roleIds)
        {
            return _next.PermissionsByRoleIdAsync(roleIds);
        }

        public Task<IDictionary<string, TRole>> RolesAsync()
        {
            return _next.RolesAsync();
        }

        public async Task DeleteRolesAsync(IEnumerable<string> roleIds)
        {
            await _next.DeleteRolesAsync(roleIds);
            await _dbContext.SaveChangesAsync();
        }

        public Task<IDictionary<string, TRole>> RolesByIdAsync(IEnumerable<string> roleIds)
        {
            return _next.RolesByIdAsync(roleIds);
        }

        public Task<IDictionary<string, IEnumerable<string>>> UsersByRoleIdAsync(IEnumerable<string> roleIds)
        {
            return _next.UsersByRoleIdAsync(roleIds);
        }
    }
}