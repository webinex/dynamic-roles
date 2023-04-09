using System;
using FluentAssertions;
using NUnit.Framework;
using Webinex.DynamicRoles.EntityFrameworkCore;
using TypeExtensions = Webinex.DynamicRoles.EntityFrameworkCore.TypeExtensions;

namespace Webinex.DynamicRoles.Tests.Unit.EfCore;

public class TypeExtensionsTests
{
    [Test]
    public void WhenClassNull_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(
            () => TypeExtensions.ClosedGenericInterface(null, typeof(IInterface<>)));
    }

    [Test]
    public void WhenInterfaceNull_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(
            () => typeof(Impl).ClosedGenericInterface(null));
    }

    [Test]
    public void WhenInterfaceArgNotInterface_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(
            () => typeof(Impl).ClosedGenericInterface(typeof(Impl)));
    }

    [Test]
    public void WhenInterfaceArgConstructed_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(
            () => typeof(Impl).ClosedGenericInterface(typeof(IInterface<string>)));
    }

    [Test]
    public void WhenClassArgNotClass_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(
            () => typeof(IInterface<string>).ClosedGenericInterface(typeof(IInterface<string>)));
    }

    [Test]
    public void WhenClassArgAbstract_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(
            () => typeof(AbstractImpl).ClosedGenericInterface(typeof(IInterface<string>)));
    }

    [Test]
    public void WhenValid_ShouldReturnCorrectResult()
    {
        var result = typeof(AbstractImpl).ClosedGenericInterface(typeof(IInterface<>));
        result.Should().Be(typeof(IInterface<string>));
    }

    private class AbstractImpl : IInterface<string>
    {
    }

    private class Impl : IInterface<string>
    {
    }

    private interface IInterface<T>
    {
    }
}