using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Webinex.DynamicRoles.EntityFrameworkCore;

namespace Webinex.DynamicRoles.Tests.Unit.EfCore;

public class RoleModelTests
{
    private Mock<IDynamicRoleModelsDefinition<TestRole, TestRolePermission, TestRoleUser>> _modelDefinitionMock;

    private RoleModel<TestRole, TestRolePermission, TestRoleUser> _subject;

    private TestRole _newRoleResult;

    [Test]
    public void RoleId_WhenValid_ShouldReturnCorrectResult()
    {
        var role = TestRole.New();
        var roleId = _subject.RoleId(role);

        roleId.Should().Be(role.RoleId.ToString());
    }

    [Test]
    public void RoleId_WhenNull_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() => _subject.RoleId(null));
    }

    [Test]
    public void RoleIdIn_WhenValid_ShouldFilterCorrectly()
    {
        var role1 = TestRole.New();
        var role2 = TestRole.New();

        var roles = new[] { role1, role2 };
        var filter = _subject.RoleIdIn(new[] { role1.RoleId.ToString() });

        var result = roles.Where(filter.Compile()).ToArray();
        result.Length.Should().Be(1);
        result.Single().Should().Be(role1);
    }

    [Test]
    public void RoleIdIn_WhenNoValues_ShouldReturnEmptyResult()
    {
        var filter = _subject.RoleIdIn(Array.Empty<string>());

        var result = new[] { TestRole.New(), TestRole.New() }.Where(filter.Compile()).ToArray();
        result.Length.Should().Be(0);
    }

    [Test]
    public void RoleIdIn_WhenNull_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() => _subject.RoleIdIn(null));
    }

    [Test]
    public void TypedRoleId_WhenValid_ShouldReturnCorrectGuid()
    {
        var role = TestRole.New();

        var result = _subject.TypedRoleId(role.RoleId.ToString());

        result.Should().BeOfType<Guid>();
        ((Guid)result).Should().Be(role.RoleId);
    }

    [Test]
    public void TypedRoleId_WhenNull_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() => _subject.TypedRoleId(null));
    }

    [Test]
    public void NewRole_WhenValid_ShouldReturnResult()
    {
        var result = _subject.NewRole(new Dictionary<string, object>());
        result.Should().Be(_newRoleResult);
    }

    [Test]
    public void NewRole_WhenValuesNull_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() => _subject.NewRole(null));
    }

    [Test]
    public void NewRole_WhenReturnNull_ShouldThrow()
    {
        _newRoleResult = null;
        Assert.Throws<InvalidOperationException>(() => _subject.NewRole(new Dictionary<string, object>()));
    }

    [SetUp]
    public void SetUp()
    {
        _newRoleResult = TestRole.New();
        _modelDefinitionMock = new Mock<IDynamicRoleModelsDefinition<TestRole, TestRolePermission, TestRoleUser>>();

        _modelDefinitionMock
            .Setup(x => x.RoleId)
            .Returns(role => role.RoleId);

        _modelDefinitionMock
            .Setup(x => x.NewRole(It.IsAny<IDictionary<string, object>>()))
            .Returns(() => _newRoleResult);

        _subject = new RoleModel<TestRole, TestRolePermission, TestRoleUser>(_modelDefinitionMock.Object);
    }
}