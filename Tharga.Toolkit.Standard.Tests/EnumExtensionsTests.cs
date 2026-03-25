using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Tharga.Toolkit.Standard.Tests;

public class EnumExtensionsTests
{
    private enum SourceEnum { Alpha, Beta, Gamma }
    private enum TargetEnum { Alpha, Beta, Gamma }
    private enum MismatchEnum { Delta }

    [Fact]
    public void MapEnum_converts_matching_enum_value()
    {
        SourceEnum.Alpha.MapEnum<TargetEnum, SourceEnum>().Should().Be(TargetEnum.Alpha);
    }

    [Fact]
    public void MapEnum_converts_collection()
    {
        var source = new[] { SourceEnum.Alpha, SourceEnum.Beta };
        var result = source.MapEnum<TargetEnum, SourceEnum>().ToList();
        result.Should().Equal(TargetEnum.Alpha, TargetEnum.Beta);
    }

    [Fact]
    public void MapEnum_throws_for_non_matching_value()
    {
        var act = () => SourceEnum.Alpha.MapEnum<MismatchEnum, SourceEnum>();
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void MapEnum_throws_for_non_enum_target_type()
    {
        var act = () => SourceEnum.Alpha.MapEnum<int, SourceEnum>();
        act.Should().Throw<InvalidOperationException>();
    }
}
