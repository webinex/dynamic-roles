using System;

namespace Webinex.DynamicRoles.Tests.Unit;

public class TestRolePermission
{
    public Guid RoleId { get; set; }
    public string Permission { get; set; }

    public static TestRolePermission New(Guid? roleId = null, string permission = null)
    {
        return new TestRolePermission
        {
            RoleId = roleId ?? Guid.NewGuid(),
            Permission = permission ?? Guid.NewGuid().ToString(),
        };
    }
}