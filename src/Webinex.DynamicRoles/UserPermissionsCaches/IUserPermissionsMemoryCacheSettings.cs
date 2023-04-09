using System;

namespace Webinex.DynamicRoles.UserPermissionsCaches
{
    internal interface IUserPermissionsMemoryCacheSettings
    {
        TimeSpan Expiration { get; }
        string Prefix { get; }
    }
}