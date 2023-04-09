using System;

namespace Webinex.DynamicRoles.Tests.Unit;

public class TestRole
{
    public Guid RoleId { get; set; }

    public static TestRole New(Guid? id = null)
    {
        return new TestRole { RoleId = id ?? Guid.NewGuid() };
    }
}