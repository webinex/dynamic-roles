using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Webinex.DynamicRoles.EntityFrameworkCore;

namespace Webinex.DynamicRoles.Tests.Unit.EfCore;

public class ExpressionsTests
{
    [Test]
    public void Contains_WhenValueAccessorNull_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(
            () => Expressions.Contains<TestRole, string>(null, Array.Empty<string>()));
    }

    [Test]
    public void Contains_WhenValuesNull_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(
            () => Expressions.Contains<TestRole, string>(role => role.RoleId.ToString(), null));
    }

    [Test]
    public void Contains_WhenEmpty_ShouldReturnCorrectExpression()
    {
        var roles = new[] { TestRole.New(), TestRole.New(), TestRole.New() };
        var filter = Expressions.Contains((TestRole role) => role.RoleId, Array.Empty<Guid>());

        var result = roles.Where(filter.Compile()).ToArray();

        result.Length.Should().Be(0);
    }

    [Test]
    public void Contains_WhenValid_ShouldReturnCorrectExpression()
    {
        var roleIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var roles = new[]
            { TestRole.New(roleIds.ElementAt(1)), TestRole.New(), TestRole.New(roleIds.ElementAt(0)) };
        var filter = Expressions.Contains((TestRole role) => role.RoleId, roleIds);

        var result = roles.Where(filter.Compile()).ToArray();

        result.Length.Should().Be(2);
        result[0].Should().Be(roles[0]);
        result[1].Should().Be(roles[2]);
    }

    [Test]
    public void Equals_WhenValueAccessorNull_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() =>
            Expressions.Equals<TestRole>(null, Guid.NewGuid().ToString()));
    }

    [Test]
    public void Equals_WhenValueNull_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() =>
            Expressions.Equals<TestRole>(x => x.RoleId.ToString(), null));
    }

    [Test]
    public void Equals_WhenValid_ShouldReturnCorrectExpression()
    {
        var role1 = TestRole.New();
        var role2 = TestRole.New();
        var roles = new[] { role1, role2 };

        var filter = Expressions.Equals<TestRole>(role => role.RoleId, role2.RoleId);

        var result = roles.Where(filter.Compile()).ToArray();

        result.Length.Should().Be(1);
        result.Single().Should().Be(role2);
    }
}