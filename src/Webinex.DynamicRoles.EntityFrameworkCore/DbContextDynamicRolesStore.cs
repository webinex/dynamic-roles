using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Webinex.DynamicRoles.Stores;

namespace Webinex.DynamicRoles.EntityFrameworkCore
{
    internal class EfCoreDynamicRolesStore<TDbContext, TRole, TRoleUser, TRolePermission>
        : IDynamicRoleStore<TRole>
        where TRole : class
        where TRoleUser : class
        where TRolePermission : class
        where TDbContext : DbContext, IDynamicRoleDbContext<TRole, TRoleUser, TRolePermission>
    {
        private readonly TDbContext _dbContext;
        private readonly IRoleModel<TRole> _roleModel;
        private readonly IRoleUserModel<TRoleUser> _roleUserModel;
        private readonly IRolePermissionModel<TRolePermission> _rolePermissionModel;

        public EfCoreDynamicRolesStore(
            TDbContext dbContext,
            IRoleModel<TRole> roleModel,
            IRoleUserModel<TRoleUser> roleUserModel,
            IRolePermissionModel<TRolePermission> rolePermissionModel)
        {
            _dbContext = dbContext;
            _roleModel = roleModel;
            _roleUserModel = roleUserModel;
            _rolePermissionModel = rolePermissionModel;
        }

        public Task<string[]> CreateAsync(IEnumerable<CreateRoleArgs> args)
        {
            args = args ?? throw new ArgumentNullException();
            return Task.FromResult(args.Select(Create).ToArray());
        }

        private string Create(CreateRoleArgs args)
        {
            var role = _roleModel.NewRole(args.Values);
            var permissions = NewPermissionModels(role, args.Permissions);
            var users = NewRoleUsersModels(role, args.UserIds);

            _dbContext.Roles.Add(role);
            _dbContext.RolePermissions.AddRange(permissions);
            _dbContext.RoleUsers.AddRange(users);

            return _roleModel.RoleId(role);
        }

        private TRolePermission[] NewPermissionModels(TRole role, string[] permissions)
        {
            var roleId = _roleModel.RoleId(role);
            return permissions.Select(permission => _rolePermissionModel.New(roleId, permission)).ToArray();
        }

        private TRoleUser[] NewRoleUsersModels(TRole role, IEnumerable<string> userIds)
        {
            var roleId = _roleModel.RoleId(role);
            return userIds.Select(userId => _roleUserModel.New(roleId, userId)).ToArray();
        }

        private TRoleUser[] CreateUserRolesModels(object userId, IEnumerable<object> roleIds)
        {
            return roleIds
                .Select(roleId => _roleUserModel.New(roleId, userId))
                .ToArray();
        }

        public async Task DeleteAsync(IEnumerable<string> roleIds)
        {
            var roles = await ByIdsAsync(roleIds);
            _dbContext.Roles.RemoveRange(roles);
        }

        private async Task<TRole[]> ByIdsAsync(IEnumerable<object> roleIds)
        {
            var filter = _roleModel.RoleIdIn(roleIds);
            return await _dbContext.Roles.Where(filter).ToArrayAsync();
        }

        public async Task<IDictionary<string, TRole>> RolesAsync()
        {
            var roles = await _dbContext.Roles.ToArrayAsync();
            var roleIds = roles.Select(role => _roleModel.RoleId(role)).ToArray();
            return roleIds.ToDictionary(
                id => id,
                id => roles.FirstOrDefault(role => _roleModel.RoleId(role).ToString() == id));
        }

        public async Task<IDictionary<string, TRole>> RolesByIdAsync(IEnumerable<string> roleIds)
        {
            roleIds = roleIds?.Distinct().ToArray() ?? throw new ArgumentNullException(nameof(roleIds));

            var roles = await ByIdsAsync(roleIds);
            return roleIds.ToDictionary(
                id => id,
                id => roles.FirstOrDefault(role => _roleModel.RoleId(role).ToString() == id));
        }

        public async Task UpdateRolesAsync(IEnumerable<UpdateRoleArgs> args)
        {
            foreach (var arg in args)
                await UpdateRoleAsync(arg);
        }

        private async Task UpdateRoleAsync(UpdateRoleArgs args)
        {
            var role = await _dbContext.Roles.FindAsync(_roleModel.TypedRoleId(args.Id));

            if (args.ShouldUpdateModel)
                _roleModel.SetRoleValues(role, args.Values);

            if (args.ShouldUpdatePermissions)
                await UpdateRolePermissionsAsync(role, args.Permissions);

            if (args.ShouldUpdateUsers)
                await UpdateRoleUsersAsync(role, args.UserIds);
        }

        private async Task UpdateRolePermissionsAsync(TRole role, string[] permissionKinds)
        {
            var roleId = _roleModel.RoleId(role);
            var permissionsInStorage = await RolePermissionsByRoleIdAsync(roleId);
            var permissionsInStorageKinds = permissionsInStorage.Select(x => _rolePermissionModel.Permission(x)).ToArray();
            var deleted = permissionsInStorage.Where(p => !permissionKinds.Contains(_rolePermissionModel.Permission(p)));
            var addedKinds = permissionKinds.Where(k => !permissionsInStorageKinds.Contains(k));
            var added = NewPermissionModels(role, addedKinds.ToArray());

            _dbContext.RolePermissions.RemoveRange(deleted);
            _dbContext.RolePermissions.AddRange(added);
        }

        private async Task<TRolePermission[]> RolePermissionsByRoleIdAsync(object roleId)
        {
            return await _dbContext.RolePermissions.Where(_rolePermissionModel.ByRoleId(roleId))
                .ToArrayAsync();
        }

        private async Task UpdateRoleUsersAsync(TRole role, string[] userIds)
        {
            var usersInStorage = await RoleUsersByRoleIdAsync(_roleModel.RoleId(role));
            var usersInStorageIds = usersInStorage.Select(x => _roleUserModel.UserId(x)).ToArray();
            var deleted = usersInStorage.Where(p => userIds.All(id =>
                !id.Equals(_roleUserModel.UserId(p), StringComparison.InvariantCulture)));

            var addedIds = userIds.Where(k => usersInStorageIds.All(uid => !k.Equals(uid)));
            var added = NewRoleUsersModels(role, addedIds.Select(x => x.ToString()));

            _dbContext.RoleUsers.RemoveRange(deleted);
            _dbContext.RoleUsers.AddRange(added);
        }

        private async Task<TRoleUser[]> RoleUsersByRoleIdAsync(object roleId)
        {
            return await _dbContext.RoleUsers.Where(_roleUserModel.ByRoleId(roleId)).ToArrayAsync();
        }

        public async Task UpdateUsersRolesAsync(IEnumerable<UpdateUserRolesArgs> args)
        {
            foreach (var arg in args)
                await UpdateUserRolesAsync(arg);
        }

        private async Task UpdateUserRolesAsync(UpdateUserRolesArgs args)
        {
            var roleUsersInStorage = await RoleUsersByUserIdAsync(args.UserId);
            var rolesIdsInStorage = roleUsersInStorage.Select(x => _roleUserModel.RoleId(x)).ToArray();
            var deleted = roleUsersInStorage.Where(p => args.RoleIds.All(roleId => !roleId.Equals(_roleUserModel.RoleId(p), StringComparison.InvariantCulture)));
            var addedIds = args.RoleIds.Where(k => rolesIdsInStorage.All(roleId => !roleId.Equals(k, StringComparison.InvariantCulture)));
            var added = CreateUserRolesModels(args.UserId, addedIds.Select(x => x.ToString()));

            _dbContext.RoleUsers.RemoveRange(deleted);
            _dbContext.RoleUsers.AddRange(added);
        }

        private async Task<TRoleUser[]> RoleUsersByUserIdAsync(object userId)
        {
            return await _dbContext.RoleUsers.Where(_roleUserModel.ByUserId(userId)).ToArrayAsync();
        }

        public async Task<IDictionary<string, IEnumerable<string>>> GetUserRolesAsync(IEnumerable<string> userIds)
        {
            userIds = userIds?.Distinct().ToArray() ?? throw new ArgumentNullException(nameof(userIds));

            var roles = await RoleUsersByUserIdsAsync(userIds);
            return Dictionary(userIds, roles, x => _roleUserModel.UserId(x), x => _roleUserModel.RoleId(x));
        }

        private async Task<TRoleUser[]> RoleUsersByUserIdsAsync(IEnumerable<object> userIds)
        {
            return await _dbContext.RoleUsers.Where(_roleUserModel.ByUserIds(userIds)).ToArrayAsync();
        }

        public async Task<IDictionary<string, IEnumerable<string>>> GetUserPermissionsAsync(IEnumerable<string> userIds)
        {
            userIds = userIds?.Distinct().ToArray() ?? throw new ArgumentNullException(nameof(userIds));

            var roleUsers = await _dbContext.RoleUsers.Where(_roleUserModel.ByUserIds(userIds)).ToArrayAsync();
            var roleIds = roleUsers.Select(roleUser => _roleUserModel.RoleId(roleUser)).ToArray();
            var rolePermissions = await _dbContext.RolePermissions
                .Where(_rolePermissionModel.RoleIdIn(roleIds)).ToArrayAsync();

            var rolesByUserId = roleUsers.ToLookup(
                roleUser => _roleUserModel.UserId(roleUser).ToLowerInvariant(),
                roleUser => _roleUserModel.RoleId(roleUser));

            var rolePermissionsByRoleId = rolePermissions.ToLookup(
                rolePermission => _rolePermissionModel.RoleId(rolePermission),
                rolePermission => _rolePermissionModel.Permission(rolePermission));

            return userIds.ToDictionary(
                userId => userId,
                userId => rolesByUserId[userId].SelectMany(roleId => rolePermissionsByRoleId[roleId]));
        }

        public async Task<IDictionary<string, IEnumerable<string>>> GetRolePermissionsAsync(IEnumerable<string> roleIds)
        {
            roleIds = roleIds?.Distinct().ToArray() ?? throw new ArgumentNullException(nameof(roleIds));
            var results = await _dbContext.RolePermissions.Where(_rolePermissionModel.RoleIdIn(roleIds))
                .ToArrayAsync();
            return Dictionary(roleIds, results, x => _rolePermissionModel.RoleId(x), x => _rolePermissionModel.Permission(x));
        }

        public async Task<IDictionary<string, IEnumerable<string>>> GetUsersByRoleIdsAsync(IEnumerable<string> roleIds)
        {
            roleIds = roleIds?.Distinct().ToArray() ?? throw new ArgumentNullException(nameof(roleIds));
            var results = await _dbContext.RoleUsers.Where(_roleUserModel.ByRoleIds(roleIds)).ToArrayAsync();
            return Dictionary(roleIds, results, x => _roleUserModel.RoleId(x), x => _roleUserModel.UserId(x));
        }

        private IDictionary<string, IEnumerable<string>> Dictionary<T>(
            IEnumerable<string> ids,
            T[] models,
            Func<T, object> idSelector,
            Func<T, object> valueIdSelector)
        {
            return ids.ToDictionary(
                id => id,
                id => models
                    .Where(model =>
                        idSelector(model).ToString().Equals(id, StringComparison.InvariantCulture))
                    .Select(x => valueIdSelector(x).ToString()));
        }
    }
}