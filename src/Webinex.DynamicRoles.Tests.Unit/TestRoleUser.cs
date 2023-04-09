using System;

namespace Webinex.DynamicRoles.Tests.Unit;

public class TestRoleUser
{
    public Guid RoleId { get; set; }
    public Guid UserId { get; set; }

    public static TestRoleUser New(Guid? roleId = null, Guid? userId = null)
    {
        return new TestRoleUser
        {
            RoleId = roleId ?? Guid.NewGuid(),
            UserId = userId ?? Guid.NewGuid(),
        };
    }
}