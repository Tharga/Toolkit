using System.Linq;
using System.Reflection;
using FluentAssertions;
using Tharga.Test.Toolkit;
using Xunit;

namespace Tharga.Toolkit.Standard.Tests;

public class DependencyTest : DependencyTestBase
{
    public DependencyTest()
        : base(Assembly.GetAssembly(typeof(ListExtensions)), GetStandardAssembliesToIgnore().Union(new[] { "mscorlib" }).ToArray())
    {
    }

    [Fact]
    public void Has_no_accidental_dependency()
    {
        //act
        var dps = GetDependencies().ToArray();

        //Assert
        dps.Should().BeEmpty();
    }
}