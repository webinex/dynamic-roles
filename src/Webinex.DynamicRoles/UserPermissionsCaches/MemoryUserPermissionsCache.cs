using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;

namespace Webinex.DynamicRoles.UserPermissionsCaches
{
    internal class MemoryUserPermissionsCache : IUserPermissionsCache
    {
        private readonly IMemoryCache _cache;
        private readonly IUserPermissionsMemoryCacheSettings _settings;

        public MemoryUserPermissionsCache(IMemoryCache cache, IUserPermissionsMemoryCacheSettings settings)
        {
            _cache = cache;
            _settings = settings;
        }

        public IDictionary<string, IEnumerable<string>> Get(IEnumerable<string> userId)
        {
            userId = userId?.Distinct().ToArray() ?? throw new ArgumentNullException(nameof(userId));
            return userId.ToDictionary(id => id, Get);
        }

        private IEnumerable<string> Get(string userId)
        {
            return _cache.Get<IEnumerable<string>>(Key(userId));
        }

        public void Set(IDictionary<string, IEnumerable<string>> permissionsByUserId)
        {
            permissionsByUserId = permissionsByUserId ?? throw new ArgumentNullException(nameof(permissionsByUserId));
            foreach (var userIdPermissions in permissionsByUserId)
            {
                Set(userIdPermissions.Key, userIdPermissions.Value);
            }
        }

        private void Set(string userId, IEnumerable<string> permissions)
        {
            userId = userId ?? throw new ArgumentNullException(nameof(userId));
            permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));

            _cache.Set(Key(userId), permissions, _settings.Expiration);
        }

        public void Revoke(IEnumerable<string> userIds)
        {
            userIds = userIds?.Distinct().ToArray() ?? throw new ArgumentNullException(nameof(userIds));
            foreach (var userId in userIds)
            {
                Revoke(userId);
            }
        }

        private void Revoke(string userId)
        {
            userId = userId ?? throw new ArgumentNullException(nameof(userId));
            
            _cache.Remove(Key(userId));
        }

        private string Key(string userId)
        {
            return $"{_settings.Prefix}-{userId}";
        }
    }
}