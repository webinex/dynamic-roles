using System.Collections.Generic;
using System.Linq;

namespace Webinex.DynamicRoles.UserPermissionsCaches
{
    internal class NullUserPermissionsCache : IUserPermissionsCache
    {
        public IDictionary<string, IEnumerable<string>> Get(IEnumerable<string> userId)
        {
            return userId.ToDictionary(x => x, x => (IEnumerable<string>)null);
        }

        public void Set(IDictionary<string, IEnumerable<string>> permissionsByUserId)
        {
        }

        public void Revoke(IEnumerable<string> userIds)
        {
        }
    }
}