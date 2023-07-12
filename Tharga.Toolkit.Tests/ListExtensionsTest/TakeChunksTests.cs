using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Tharga.Toolkit.Tests.ListExtensionsTest
{
    public class TakeChunksTests
    {
        [Fact]
        public void GetChunksFromCollection()
        {
            //Arrange
            var list = new Collection<string> { "A", "B", "C" };

            //Act
            var item = list.TakeChunks(2);

            //Assert
            item.Should().NotBeNullOrEmpty();
            item.SelectMany(x => x).Should().HaveCount(list.Count);
            item.Should().HaveCount(2);
        }

        [Fact]
        public void GetChunksWithOneInEach()
        {
            //Arrange
            var list = new Collection<string> { "A", "B", "C" };

            //Act
            var item = list.TakeChunks(1);

            //Assert
            item.Should().NotBeNullOrEmpty();
            item.SelectMany(x => x).Should().HaveCount(list.Count);
            item.Should().HaveCount(list.Count);
        }

        [Fact]
        public void GetChunksWithOneLargerValue()
        {
            //Arrange
            var list = new Collection<string> { "A", "B", "C" };

            //Act
            var item = list.TakeChunks(100);

            //Assert
            item.Should().NotBeNullOrEmpty();
            item.SelectMany(x => x).Should().HaveCount(list.Count);
            item.Should().HaveCount(1);
        }

        [Fact]
        public void GetChunksWithExactValue()
        {
            //Arrange
            var list = new Collection<string> { "A", "B", "C" };

            //Act
            var item = list.TakeChunks(list.Count);

            //Assert
            item.Should().NotBeNullOrEmpty();
            item.SelectMany(x => x).Should().HaveCount(list.Count);
            item.Should().HaveCount(1);
        }

        [Fact]
        public void GetChunksWithEvenNumber()
        {
            //Arrange
            var list = new Collection<string> { "A", "B", "C", "D" };

            //Act
            var item = list.TakeChunks(2);

            //Assert
            item.Should().NotBeNullOrEmpty();
            item.SelectMany(x => x).Should().HaveCount(list.Count);
            item.Should().HaveCount(2);
        }

        [Fact]
        public void GetChunksFromEmptyList()
        {
            //Arrange
            var list = new List<string>();

            //Act
            var item = list.TakeChunks(1);

            //Assert
            item.Should().BeEmpty();
            item.SelectMany(x => x).Should().HaveCount(list.Count);
        }

        [Fact]
        public void TakeAllButLast_when_null()
        {
            // Arrange
            List<string> strings = null;

            // Act
            var result = strings.TakeChunks(10);

            // Assert
            result.Should().BeNull();
        }
    }
}