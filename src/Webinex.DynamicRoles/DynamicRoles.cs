using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webinex.DynamicRoles.Stores;
using Webinex.DynamicRoles.UserPermissionsCaches;

namespace Webinex.DynamicRoles
{
    internal class DynamicRoles<TRole> : IDynamicRoles<TRole>
    {
        private readonly IDynamicRoleStore<TRole> _dynamicRoleStore;
        private readonly IPermissionConfigurationService _permissionConfigurationService;
        private readonly IPermissionsValidator _permissionsValidator;
        private readonly IUserPermissionsCache _userPermissionsCache;

        public DynamicRoles(
            IDynamicRoleStore<TRole> dynamicRoleStore,
            IPermissionConfigurationService permissionConfigurationService,
            IPermissionsValidator permissionsValidator,
            IUserPermissionsCache userPermissionsCache)
        {
            _dynamicRoleStore = dynamicRoleStore;
            _permissionConfigurationService = permissionConfigurationService;
            _permissionsValidator = permissionsValidator;
            _userPermissionsCache = userPermissionsCache;
        }

        public async Task<PermissionsConfiguration> PermissionsConfigurationAsync()
        {
            return await _permissionConfigurationService.GetAsync();
        }

        public async Task<string[]> CreateRolesAsync(IEnumerable<CreateRoleArgs> args)
        {
            args = args?.ToArray() ?? throw new ArgumentNullException(nameof(args));
            var permissions = args.SelectMany(x => x.Permissions ?? Array.Empty<string>()).Distinct().ToArray();
            await _permissionsValidator.ValidateAndThrowAsync(permissions);

            var result = await _dynamicRoleStore.CreateAsync(args);
            RevokeUserPermissionsCache(args);
            return result;
        }

        private void RevokeUserPermissionsCache(IEnumerable<CreateRoleArgs> args)
        {
            var userIds = args.SelectMany(x => x.UserIds ?? Array.Empty<string>()).Distinct().ToArray();
            _userPermissionsCache.Revoke(userIds);
        }

        public async Task UpdateRolesAsync(IEnumerable<UpdateRoleArgs> argsEnumerable)
        {
            var args = argsEnumerable?.ToArray() ?? throw new ArgumentNullException(nameof(argsEnumerable));
            await ValidateRolesAsync(args.Select(x => x.Id));
            await _permissionsValidator.ValidateAndThrowAsync(args.SelectMany(x => x.Permissions));
            await _dynamicRoleStore.UpdateRolesAsync(args);
            await RevokeUserPermissionsCacheAsync(args);
        }

        private async Task RevokeUserPermissionsCacheAsync(UpdateRoleArgs[] args)
        {
            var roleIds = args.Select(x => x.Id).ToArray();
            var userByRoleId = await _dynamicRoleStore.GetUsersByRoleIdsAsync(roleIds);
            var existingUsers = userByRoleId.Values.SelectMany(x => x);
            var newUsers = args.SelectMany(x => x.UserIds ?? Array.Empty<string>()).Distinct().ToArray();
            _userPermissionsCache.Revoke(existingUsers.Concat(newUsers).Distinct().ToArray());
        }

        public async Task UpdateUsersRolesAsync(IEnumerable<UpdateUserRolesArgs> args)
        {
            args = args?.ToArray() ?? throw new ArgumentNullException(nameof(args));
            await ValidateRolesAsync(args.SelectMany(x => x.RoleIds ?? Array.Empty<string>()));
            await _dynamicRoleStore.UpdateUsersRolesAsync(args);
            RevokeUserPermissionsCache(args);
        }

        private void RevokeUserPermissionsCache(IEnumerable<UpdateUserRolesArgs> args)
        {
            var userIds = args.Select(x => x.UserId).Distinct().ToArray();
            _userPermissionsCache.Revoke(userIds);
        }

        public async Task<IDictionary<string, IEnumerable<string>>> RolesByUserIdAsync(IEnumerable<string> userIds)
        {
            return await _dynamicRoleStore.GetUserRolesAsync(userIds);
        }

        public async Task<IDictionary<string, IEnumerable<string>>> PermissionsByUserIdAsync(
            IEnumerable<string> userIds)
        {
            userIds = userIds?.Distinct().ToArray() ?? throw new ArgumentNullException(nameof(userIds));

            var cacheResult = _userPermissionsCache.Get(userIds);
            var notFoundInCache = cacheResult
                .Where(x => x.Value == null)
                .Select(x => x.Key)
                .ToArray();

            var fromStore = await _dynamicRoleStore.GetUserPermissionsAsync(notFoundInCache);
            _userPermissionsCache.Set(fromStore);
            return new Dictionary<string, IEnumerable<string>>(
                cacheResult.Where(x => x.Value != null).Concat(fromStore));
        }

        public async Task<IDictionary<string, IEnumerable<string>>> PermissionsByRoleIdAsync(
            IEnumerable<string> roleIds)
        {
            return await _dynamicRoleStore.GetRolePermissionsAsync(roleIds);
        }

        public async Task<IDictionary<string, TRole>> RolesAsync()
        {
            return await _dynamicRoleStore.RolesAsync();
        }

        public async Task DeleteRolesAsync(IEnumerable<string> roleIds)
        {
            roleIds = roleIds?.Distinct().ToArray() ?? throw new ArgumentNullException(nameof(roleIds));
            await ValidateRolesAsync(roleIds);

            var roleUsers = await GetUsersAssignedToRolesAsync(roleIds);
            await _dynamicRoleStore.DeleteAsync(roleIds);
            _userPermissionsCache.Revoke(roleUsers);
        }

        private async Task<IEnumerable<string>> GetUsersAssignedToRolesAsync(IEnumerable<string> roleIds)
        {
            var usersByRoleId = await UsersByRoleIdAsync(roleIds);
            return usersByRoleId.SelectMany(x => x.Value).Distinct().ToArray();
        }

        public async Task<IDictionary<string, TRole>> RolesByIdAsync(IEnumerable<string> roleIds)
        {
            roleIds = roleIds?.Distinct().ToArray() ?? throw new ArgumentNullException(nameof(roleIds));

            return await _dynamicRoleStore.RolesByIdAsync(roleIds);
        }

        public async Task<IDictionary<string, IEnumerable<string>>> UsersByRoleIdAsync(IEnumerable<string> roleIds)
        {
            roleIds = roleIds?.Distinct().ToArray() ?? throw new ArgumentNullException(nameof(roleIds));
            return await _dynamicRoleStore.GetUsersByRoleIdsAsync(roleIds);
        }

        private async Task ValidateRolesAsync(IEnumerable<string> roleIds)
        {
            roleIds = roleIds?.Select(x => x.ToLowerInvariant()).ToArray() ??
                      throw new ArgumentNullException(nameof(roleIds));

            var roles = await _dynamicRoleStore.RolesByIdAsync(roleIds);
            var roleIdsFound = roles.Keys.Select(x => x.ToLowerInvariant());
            var roleIdsNotFound = roleIdsFound.Except(roleIds).ToArray();

            if (roleIdsNotFound.Any())
                throw new InvalidOperationException($"Roles not found: {string.Join(", ", roleIdsNotFound)}");
        }
    }
}