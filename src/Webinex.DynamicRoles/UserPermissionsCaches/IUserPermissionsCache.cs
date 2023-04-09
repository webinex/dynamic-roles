using System.Collections.Generic;

namespace Webinex.DynamicRoles.UserPermissionsCaches
{
    internal interface IUserPermissionsCache
    {
        IDictionary<string, IEnumerable<string>> Get(IEnumerable<string> userId);

        void Set(IDictionary<string, IEnumerable<string>> permissionsByUserId);

        void Revoke(IEnumerable<string> userIds);
    }
}