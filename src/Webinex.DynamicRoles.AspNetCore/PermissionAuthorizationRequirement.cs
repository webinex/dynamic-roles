using System;
using Microsoft.AspNetCore.Authorization;

namespace Webinex.DynamicRoles.AspNetCore
{
    internal class PermissionAuthorizationRequirement : IAuthorizationRequirement
    {
        public PermissionAuthorizationRequirement(Func<string[], bool> condition)
        {
            Condition = condition ?? throw new ArgumentNullException(nameof(condition));
        }

        public Func<string[], bool> Condition { get; }
    }
}