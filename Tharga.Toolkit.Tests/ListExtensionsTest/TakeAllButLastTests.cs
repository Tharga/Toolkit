using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Tharga.Toolkit.Tests.ListExtensionsTest
{
    public class TakeAllButLastTests
    {
        [Fact]
        public void TakeAllButLast()
        {
            //Arrange
            var strings = new List<string> {"A", "B", "C"};

            //Act
            var result = strings.TakeAllButLast().ToList();

            //Assert
            result.Should().HaveCount(strings.Count - 1);
            result[0].Should().Be(strings[0]);
            result[1].Should().Be(strings[1]);
        }

        [Fact]
        public void TakeAllButLast_when_contains_only_one()
        {
            // Arrange
            var strings = new List<string> {"A"};

            // Act
            var result = strings.TakeAllButLast().ToList();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void TakeAllButLast_when_empty()
        {
            //Arrange
            var strings = new List<string>();

            //Act
            var result = strings.TakeAllButLast().ToList();

            //Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void TakeAllButLast_when_null()
        {
            // Arrange
            List<string> strings = null;

            // Act
            var result = strings.TakeAllButLast();

            // Assert
            result.Should().BeNull();
        }
    }
}