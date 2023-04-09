using System;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Webinex.DynamicRoles.EntityFrameworkCore;

namespace Webinex.DynamicRoles.Tests.Unit.EfCore;

public class RoleUserModelTests
{
    private Mock<IDynamicRoleModelsDefinition<TestRole, TestRolePermission, TestRoleUser>> _modelDefinitionMock;

    private RoleUserModel<TestRole, TestRolePermission, TestRoleUser> _subject;

    private TestRoleUser _newResult;

    [Test]
    public void New_WhenRoleIdNull_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() => _subject.New(null, Guid.NewGuid()));
    }

    [Test]
    public void New_WhenUserIdNull_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() => _subject.New(Guid.NewGuid(), null));
    }

    [Test]
    public void New_WhenReturnNull_ShouldThrow()
    {
        _newResult = null;
        Assert.Throws<InvalidOperationException>(() => _subject.New(Guid.NewGuid(), Guid.NewGuid()));
    }

    [Test]
    public void New_WhenValid_ShouldReturnResult()
    {
        _newResult = TestRoleUser.New();

        var result = _subject.New(Guid.NewGuid(), Guid.NewGuid());
        result.Should().Be(_newResult);
    }

    [Test]
    public void UserId_WhenArgNull_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() => _subject.UserId(null));
    }

    [Test]
    public void UserId_WhenValid_ShouldReturnId()
    {
        var roleUser = TestRoleUser.New();
        var result = _subject.UserId(roleUser);

        result.Should().Be(roleUser.UserId.ToString());
    }

    [Test]
    public void ByUserId_WhenArgNull_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() => _subject.ByUserId(null));
    }

    [Test]
    public void ByUserId_WhenValid_ShouldFilterCorrectly()
    {
        var roleUser1 = TestRoleUser.New();
        var roleUser2 = TestRoleUser.New();
        var roleUsers = new[] { roleUser1, roleUser2 };

        var result = roleUsers.Where(_subject.ByRoleId(roleUser1.RoleId.ToString()).Compile()).ToArray();

        result.Length.Should().Be(1);
        result.Single().Should().Be(roleUser1);
    }

    [Test]
    public void ByUserIds_WhenArgNull_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() => _subject.ByUserIds(null));
    }

    [Test]
    public void ByUserIds_WhenEmpty_ShouldReturnEmptyResult()
    {
        var roleUser1 = TestRoleUser.New();
        var roleUser2 = TestRoleUser.New();
        var roleUsers = new[] { roleUser1, roleUser2 };

        var result = roleUsers.Where(_subject.ByUserIds(Array.Empty<string>()).Compile()).ToArray();

        result.Length.Should().Be(0);
    }

    [Test]
    public void ByUserIds_WhenValid_ShouldFilterCorrectly()
    {
        var roleUser1 = TestRoleUser.New();
        var roleUser2 = TestRoleUser.New();
        var roleUser3 = TestRoleUser.New();
        var roleUsers = new[] { roleUser1, roleUser2, roleUser3 };

        var userIds = new[] { roleUser1.UserId.ToString(), roleUser3.UserId.ToString() };
        var result = roleUsers.Where(_subject.ByUserIds(userIds).Compile()).ToArray();

        result.Length.Should().Be(2);
        result.ElementAt(0).Should().Be(roleUser1);
        result.ElementAt(1).Should().Be(roleUser3);
    }

    [SetUp]
    public void SetUp()
    {
        _modelDefinitionMock = new Mock<IDynamicRoleModelsDefinition<TestRole, TestRolePermission, TestRoleUser>>();
        _subject = new RoleUserModel<TestRole, TestRolePermission, TestRoleUser>(_modelDefinitionMock.Object);
        _newResult = TestRoleUser.New();

        _modelDefinitionMock
            .Setup(x => x.RoleUserRoleId)
            .Returns(roleUser => roleUser.RoleId);

        _modelDefinitionMock
            .Setup(x => x.RoleUserUserId)
            .Returns(roleUser => roleUser.UserId);

        _modelDefinitionMock
            .Setup(x => x.NewRoleUser(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(() => _newResult);
    }
}