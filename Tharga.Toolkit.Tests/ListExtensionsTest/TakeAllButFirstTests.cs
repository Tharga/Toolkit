using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Tharga.Toolkit.Tests.ListExtensionsTest
{
    public class TakeAllButFirstTests
    {
        [Fact]
        public void TakeAllButFirst()
        {
            //Arrange
            var strings = new List<string> { "A", "B", "C" };

            //Act
            var result = strings.TakeAllButFirst().ToList();

            //Assert
            result.Should().HaveCount(strings.Count - 1);
            result[0].Should().Be(strings[1]);
            result[1].Should().Be(strings[2]);
        }

        [Fact]
        public void TakeAllButFirst_when_contains_only_one()
        {
            // Arrange
            var strings = new List<string> { "A" };

            // Act
            var result = strings.TakeAllButFirst().ToList();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void TakeAllButFirst_when_empty()
        {
            //Arrange
            var strings = new List<string>();

            //Act
            var result = strings.TakeAllButFirst().ToList();

            //Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void TakeAllButFirst_when_null()
        {
            // Arrange
            List<string> strings = null;

            // Act
            var result = strings.TakeAllButFirst();

            // Assert
            result.Should().BeNull();
        }
    }
}