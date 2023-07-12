using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Tharga.Test.Toolkit;
using Tharga.Toolkit.Tests.Assignment.Entities;
using Xunit;

namespace Tharga.Toolkit.Tests.Assignment
{
    public class AssignmentTester
    {
        private readonly Fixture _fixture;

        public AssignmentTester()
        {
            _fixture = new Fixture();
            _fixture.Register(() => ESomeEnum.B);
            _fixture.Register(() => true);
        }

        [Fact]
        public void TextNotAssigned()
        {
            //Arrange

            //Act
            var dto = _fixture.Build<SomeDto>().With(x => x.Text, default(string)).Create();

            //Assert
            var assignmentIssues = dto.AssignmentIssues().ToArray();
            assignmentIssues.Should().HaveCount(1);
            dto.IsAssigned().Should().BeFalse();
        }

        [Fact]
        public void IntNotAssigned()
        {
            //Arrange

            //Act
            var dto = _fixture.Build<SomeDto>().With(x => x.Number, default(int)).Create();

            //Assert
            var assignmentIssues = dto.AssignmentIssues().ToArray();
            assignmentIssues.Should().HaveCount(1);
            assignmentIssues.First().Message.Should().Be("Value of type Int32 in 'SomeDto.Number' is not assigned.");
            dto.IsAssigned().Should().BeFalse();
        }

        [Fact]
        public void DateTimeNotAssigned()
        {
            //Arrange

            //Act
            var dto = _fixture.Build<SomeDto>().With(x => x.DateTime, default(DateTime)).Create();

            //Assert
            var assignmentIssues = dto.AssignmentIssues().ToArray();
            assignmentIssues.Should().HaveCount(1);
            assignmentIssues.First().Message.Should().Be("Value of type DateTime in 'SomeDto.DateTime' is not assigned.");
            dto.IsAssigned().Should().BeFalse();
        }

        [Fact]
        public void UriNotAssigned()
        {
            //Arrange

            //Act
            var dto = _fixture.Build<SomeDto>().With(x => x.Uri, default(Uri)).Create();

            //Assert
            var assignmentIssues = dto.AssignmentIssues().ToArray();
            assignmentIssues.Should().HaveCount(1);
            assignmentIssues.First().Message.Should().Be("The field 'SomeDto.Uri' is not assigned.");
            dto.IsAssigned().Should().BeFalse();
        }

        [Fact]
        public void TimeSpanNotAssigned()
        {
            //Arrange

            //Act
            var dto = _fixture.Build<SomeDto>().With(x => x.TimeSpan, default(TimeSpan)).Create();

            //Assert
            var assignmentIssues = dto.AssignmentIssues().ToArray();
            assignmentIssues.Should().HaveCount(1);
            assignmentIssues.First().Message.Should().Be("Value of type TimeSpan in 'SomeDto.TimeSpan' is not assigned.");
            dto.IsAssigned().Should().BeFalse();
        }

        [Fact]
        public void DecimalTimeNotAssigned()
        {
            //Arrange

            //Act
            var dto = _fixture.Build<SomeDto>().With(x => x.Decimal, default(decimal)).Create();

            //Assert
            var assignmentIssues = dto.AssignmentIssues().ToArray();
            assignmentIssues.Should().HaveCount(1);
            assignmentIssues.First().Message.Should().Be("Value of type Decimal in 'SomeDto.Decimal' is not assigned.");
            dto.IsAssigned().Should().BeFalse();
        }

        [Fact]
        public void DoubleTimeNotAssigned()
        {
            //Arrange

            //Act
            var dto = _fixture.Build<SomeDto>().With(x => x.Double, default(double)).Create();

            //Assert
            var assignmentIssues = dto.AssignmentIssues().ToArray();
            assignmentIssues.Should().HaveCount(1);
            assignmentIssues.First().Message.Should().Be("Value of type Double in 'SomeDto.Double' is not assigned.");
            dto.IsAssigned().Should().BeFalse();
        }

        [Fact]
        public void ArrayNotAssigned()
        {
            //Arrange

            //Act
            var dto = _fixture.Build<SomeDto>().With(x => x.Texts1, default(string[])).Create();

            //Assert
            var assignmentIssues = dto.AssignmentIssues().ToArray();
            assignmentIssues.Should().HaveCount(1);
            assignmentIssues.First().Message.Should().Be("The field 'SomeDto.Texts1' is not assigned.");
            dto.IsAssigned().Should().BeFalse();
        }

        [Fact]
        public void ListNotAssigned()
        {
            //Arrange

            //Act
            var dto = _fixture.Build<SomeDto>().With(x => x.Texts2, default(List<string>)).Create();

            //Assert
            var assignmentIssues = dto.AssignmentIssues().ToArray();
            assignmentIssues.Should().HaveCount(1);
            assignmentIssues.First().Message.Should().Be("The field 'SomeDto.Texts2' is not assigned.");
            dto.IsAssigned().Should().BeFalse();
        }

        [Fact]
        public void CollectionNotAssigned()
        {
            //Arrange

            //Act
            var dto = _fixture.Build<SomeDto>().With(x => x.Texts3, default(Collection<string>)).Create();

            //Assert
            var assignmentIssues = dto.AssignmentIssues().ToArray();
            assignmentIssues.Should().HaveCount(1);
            assignmentIssues.First().Message.Should().Be("The field 'SomeDto.Texts3' is not assigned.");
            dto.IsAssigned().Should().BeFalse();
        }

        [Fact]
        public void EnumerableInterfaceNotAssigned()
        {
            //Arrange

            //Act
            var dto = _fixture.Build<SomeDto>().With(x => x.Texts4, default(IEnumerable<string>)).Create();

            //Assert
            var assignmentIssues = dto.AssignmentIssues().ToArray();
            assignmentIssues.Should().HaveCount(1);
            assignmentIssues.First().Message.Should().Be("The field 'SomeDto.Texts4' is not assigned.");
            dto.IsAssigned().Should().BeFalse();
        }

        [Fact]
        public void DictionaryNotAssigned()
        {
            //Arrange

            //Act
            var dto = _fixture.Build<SomeDto>().With(x => x.Texts5, default(Dictionary<string,string>)).Create();

            //Assert
            var assignmentIssues = dto.AssignmentIssues().ToArray();
            assignmentIssues.Should().HaveCount(1);
            assignmentIssues.First().Message.Should().Be("The field 'SomeDto.Texts5' is not assigned.");
            dto.IsAssigned().Should().BeFalse();
        }

        [Fact]
        public void DictionaryInterfaceNotAssigned()
        {
            //Arrange

            //Act
            var dto = _fixture.Build<SomeDto>().With(x => x.Texts6, default(IDictionary<string, string>)).Create();

            //Assert
            var assignmentIssues = dto.AssignmentIssues().ToArray();
            assignmentIssues.Should().HaveCount(1);
            assignmentIssues.First().Message.Should().Be("The field 'SomeDto.Texts6' is not assigned.");
            dto.IsAssigned().Should().BeFalse();
        }

        [Fact]
        public void ListInterfaceNotAssigned()
        {
            //Arrange

            //Act
            var dto = _fixture.Build<SomeDto>().With(x => x.Texts7, default(IList<string>)).Create();

            //Assert
            var assignmentIssues = dto.AssignmentIssues().ToArray();
            assignmentIssues.Should().HaveCount(1);
            assignmentIssues.First().Message.Should().Be("The field 'SomeDto.Texts7' is not assigned.");
            dto.IsAssigned().Should().BeFalse();
        }

        [Fact]
        public void GenericNotAssigned()
        {
            //Arrange

            //Act
            var dto = _fixture.Build<SomeDto>().With(x => x.SomeGeneric, default(SomeGeneric<string>)).Create();

            //Assert
            var assignmentIssues = dto.AssignmentIssues().ToArray();
            assignmentIssues.Should().HaveCount(1);
            assignmentIssues.First().Message.Should().Be("The field 'SomeDto.SomeGeneric' is not assigned.");
            dto.IsAssigned().Should().BeFalse();
        }

        [Fact]
        public void BooleanNotAssigned()
        {
            //Arrange

            //Act
            var dto = _fixture.Build<SomeDto>().With(x => x.TrueOrFalse, default(bool)).Create();

            //Assert
            var assignmentIssues = dto.AssignmentIssues().ToArray();
            assignmentIssues.Should().HaveCount(1);
            assignmentIssues.First().Message.Should().Be("Value of type Boolean in 'SomeDto.TrueOrFalse' is not assigned.");
            dto.IsAssigned().Should().BeFalse();
        }

        [Fact]
        public void EnumAssigned()
        {
            //Arrange

            //Act
            var dto = _fixture.Build<SomeDto>().With(x => x.SomeEnum, default(ESomeEnum)).Create();

            //Assert
            var assignmentIssues = dto.AssignmentIssues().ToArray();
            assignmentIssues.Should().HaveCount(1);
            assignmentIssues.First().Message.Should().Be("Value of type ESomeEnum in 'SomeDto.SomeEnum' is not assigned.");
            dto.IsAssigned().Should().BeFalse();
        }

        [Fact]
        public void SubTypeTimeNotAssigned()
        {
            //Arrange

            //Act
            var dto = _fixture.Build<SomeDto>().With(x => x.Sub, default(SomeSubDto)).Create();

            //Assert
            var assignmentIssues = dto.AssignmentIssues().ToArray();
            assignmentIssues.Should().HaveCount(1);
            assignmentIssues.First().Message.Should().Be("The field 'SomeDto.Sub' is not assigned.");
            dto.IsAssigned().Should().BeFalse();
        }

        [Fact]
        public void IgnoreValueNotAssigned()
        {
            //Arrange

            //Act
            var dto = _fixture.Build<SomeDto>().With(x => x.Double, default(double)).Create();

            //Assert
            var assignmentIssues = dto.AssignmentIssues(new[] { "SomeDto.Double" }).ToArray();
            assignmentIssues.Should().BeEmpty();
            dto.IsAssigned().Should().BeFalse();
        }

        [Fact]
        public void IgnoreFieldNotAssigned()
        {
            //Arrange

            //Act
            var dto = _fixture.Build<SomeDto>().With(x => x.Sub, default(SomeSubDto)).Create();

            //Assert
            var assignmentIssues = dto.AssignmentIssues(new[] {"SomeDto.Sub"}).ToArray();
            assignmentIssues.Should().BeEmpty();
            dto.IsAssigned().Should().BeFalse();
        }

        [Fact]
        public void Test()
        {
            //Arrange

            //Act
            var dto = _fixture.Build<SomeDto>().Create();

            //Assert
            var assignmentIssues = dto.AssignmentIssues().ToArray();
            assignmentIssues.Should().BeEmpty(assignmentIssues.FirstOrDefault()?.Message);
            dto.IsAssigned().Should().BeTrue();
        }
    }
}