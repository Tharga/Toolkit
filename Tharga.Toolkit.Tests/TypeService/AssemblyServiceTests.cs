using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using FluentAssertions;
using Tharga.Toolkit.TypeService;
using Xunit;

namespace Tharga.Toolkit.Tests.TypeService;

public class AssemblyServiceTests
{
    [Fact]
    public void GetAssemblies()
    {
        //Act
        var assemblies = AssemblyService.GetAssemblies();

        //Assert
        var names = assemblies.Select(x => x.GetName().Name).ToArray();
        if (names.Length == 1) names.Should().Contain("ReSharperTestRunner");
        if (names.Length == 5) names.Should().Contain("nCrunch.Common.DotNetCore");
    }

    [Fact]
    public void GetAssembliesByGenericType()
    {
        //Act
        var assemblies = AssemblyService.GetAssemblies<TypeMissingException>();

        //Assert
        var names = assemblies.Select(x => x.GetName().Name).ToArray();
        names.Should().HaveCount(4);
        names.Should().Contain("Tharga.Toolkit");
    }


    [Fact]
    public void GetAssembliesByType()
    {
        //Act
        var assemblies = AssemblyService.GetAssemblies(Assembly.GetAssembly(typeof(TypeMissingException)));

        //Assert
        var names = assemblies.Select(x => x.GetName().Name).ToArray();
        names.Should().HaveCount(4);
        names.Should().Contain("Tharga.Toolkit");
    }

    [Fact]
    public void GetTypesByBaseType()
    {
        //Act
        var types = AssemblyService.GetTypes<TypeMissingException>(baseAssembly: Assembly.GetAssembly(typeof(TypeMissingException)));

        //Assert
        types.Should().HaveCount(1);
    }

    [Fact]
    public void GetTypes()
    {
        //Act
        var types = AssemblyService.GetTypes(baseAssembly: Assembly.GetAssembly(typeof(TypeMissingException)));

        //Assert
        types.Should().HaveCount(138);
    }

    [Fact]
    public void LoadCache()
    {
        //Arrange
        var sut = new AssemblyService();

        //Act
        sut.LoadTypes("A", _ => true, baseAssembly: Assembly.GetAssembly(typeof(TypeMissingException)));

        //Assert
        var types = sut.GetTypes("A");
        types.Should().HaveCount(138);
    }

    [Fact]
    public void GetTypesWithCache()
    {
        //Arrange
        var sut = new AssemblyService();

        //Act
        var types = sut.GetTypes("A", _ => true, baseAssembly: Assembly.GetAssembly(typeof(TypeMissingException)));

        //Assert
        types.Should().HaveCount(138);
        sut.GetTypes("A").Should().HaveCount(138);
    }

}