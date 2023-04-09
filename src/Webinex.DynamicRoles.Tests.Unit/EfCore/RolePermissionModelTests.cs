using System;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Webinex.DynamicRoles.EntityFrameworkCore;

namespace Webinex.DynamicRoles.Tests.Unit.EfCore;

public class RolePermissionModelTests
{
    private Mock<IDynamicRoleModelsDefinition<TestRole, TestRolePermission, TestRoleUser>> _modelDefinitionMock;

    private RolePermissionModel<TestRole, TestRolePermission, TestRoleUser> _subject;

    private TestRolePermission _newResult;

    [Test]
    public void New_WhenRoleIdNull_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() => _subject.New(null, Guid.NewGuid().ToString()));
    }

    [Test]
    public void New_WhenUserIdNull_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() => _subject.New(Guid.NewGuid().ToString(), null));
    }

    [Test]
    public void New_WhenReturnNull_ShouldThrow()
    {
        _newResult = null;
        Assert.Throws<InvalidOperationException>(() =>
            _subject.New(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
    }

    [Test]
    public void New_WhenValid_ShouldReturnResult()
    {
        _newResult = TestRolePermission.New();

        var result = _subject.New(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        result.Should().Be(_newResult);
    }

    [Test]
    public void Permission_WhenNull_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() => _subject.Permission(null));
    }

    [Test]
    public void Permission_WhenValid_ReturnValue()
    {
        var permission = TestRolePermission.New();

        var result = _subject.Permission(permission);

        result.Should().Be(permission.Permission);
    }

    [Test]
    public void ByRoleId_WhenNull_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() => _subject.ByRoleId(null));
    }

    [Test]
    public void ByRoleId_WhenValid_ShouldFilterCorrectly()
    {
        var permission1 = TestRolePermission.New();
        var permission2 = TestRolePermission.New();
        var permission3 = TestRolePermission.New();
        var input = new[] { permission1, permission2, permission3 };

        var filter = _subject.ByRoleId(permission3.RoleId);
        var result = input.Where(filter.Compile()).ToArray();

        result.Length.Should().Be(1);
        result.Single().Should().Be(permission3);
    }

    [Test]
    public void RoleIdIn_WhenNull_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() => _subject.RoleIdIn(null));
    }

    [Test]
    public void RoleIdIn_WhenEmpty_ShouldReturnEmpty()
    {
        var permission1 = TestRolePermission.New();
        var permission2 = TestRolePermission.New();
        var permission3 = TestRolePermission.New();
        var input = new[] { permission1, permission2, permission3 };

        var filter = _subject.RoleIdIn(Array.Empty<string>());
        var result = input.Where(filter.Compile()).ToArray();

        result.Length.Should().Be(0);
    }

    [Test]
    public void RoleIdIn_WhenValid_ShouldFilterCorrectly()
    {
        var permission1 = TestRolePermission.New();
        var permission2 = TestRolePermission.New();
        var permission3 = TestRolePermission.New();
        var permissions = new[] { permission1, permission2, permission3 };
        var ids = new[] { permission1.RoleId.ToString(), permission3.RoleId.ToString() };

        var filter = _subject.RoleIdIn(ids);
        var result = permissions.Where(filter.Compile()).ToArray();

        result.Length.Should().Be(2);
        result.ElementAt(0).Should().Be(permission1);
        result.ElementAt(1).Should().Be(permission3);
    }

    [Test]
    public void RoleId_WhenNull_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() => _subject.RoleId(null));
    }

    [Test]
    public void RoleId_WhenValid_ShouldReturnCorrectValue()
    {
        var permission = TestRolePermission.New();
        var result = _subject.RoleId(permission);
        result.Should().Be(permission.RoleId.ToString());
    }

    [SetUp]
    public void SetUp()
    {
        _modelDefinitionMock = new Mock<IDynamicRoleModelsDefinition<TestRole, TestRolePermission, TestRoleUser>>();
        _subject = new RolePermissionModel<TestRole, TestRolePermission, TestRoleUser>(_modelDefinitionMock.Object);
        _newResult = TestRolePermission.New();

        _modelDefinitionMock
            .Setup(x => x.RolePermissionRoleId)
            .Returns(rolePermission => rolePermission.RoleId);

        _modelDefinitionMock
            .Setup(x => x.RolePermissionPermission)
            .Returns(rolePermission => rolePermission.Permission);

        _modelDefinitionMock
            .Setup(x => x.NewRolePermission(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(() => _newResult);
    }
}