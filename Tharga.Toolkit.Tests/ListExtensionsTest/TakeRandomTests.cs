using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Tharga.Toolkit.Tests.ListExtensionsTest
{
    public class TakeRandomTests
    {
        [Fact]
        public void GetRandomItemFromCollection()
        {
            //Arrange
            var list = new Collection<string> { "A", "B", "C" };

            //Act
            var item = list.TakeRandom();

            //Assert
            item.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void GetRandomItemFromEmptyList()
        {
            //Arrange
            var list = new List<string>();

            //Act
            var item = list.TakeRandom();

            //Assert
            item.Should().BeNull();
        }

        [Fact]
        public void GetRandomItemFromList()
        {
            //Arrange
            var list = new List<string> { "A", "B", "C" };

            //Act
            var item = list.TakeRandom();

            //Assert
            item.Should().NotBeEmpty();
        }

        [Fact]
        public void GetRandomItemFromListWithOneEntity()
        {
            //Arrange
            var list = new List<string> { "A" };

            //Act
            var item = list.TakeRandom();

            //Assert
            item.Should().Be(list.Single());
        }
    }
}