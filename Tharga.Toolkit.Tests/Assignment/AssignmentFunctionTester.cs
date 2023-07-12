using System.Linq;
using AutoFixture;
using FluentAssertions;
using Tharga.Test.Toolkit;
using Tharga.Toolkit.Tests.Assignment.Entities;
using Xunit;

namespace Tharga.Toolkit.Tests.Assignment
{
    public class AssignmentFunctionTester
    {
        private readonly Fixture _fixture;

        public AssignmentFunctionTester()
        {
            _fixture = new Fixture();
            _fixture.Register(() => ESomeEnum.B);
            _fixture.Register(() => true);
        }

        [Fact]
        public void ConvertToDto()
        {
            //Arrange
            var entity = _fixture.Build<SomeEntity>().Create();
            var ignoreProperties = new string[] { };
            _fixture.Register(() => true);

            //Act
            var dto = entity.ToDto();

            //Assert
            var assignmentIssues = dto.AssignmentIssues(ignoreProperties).ToArray();
            assignmentIssues.Should().BeEmpty();
            dto.IsAssigned(ignoreProperties).Should().BeTrue();
        }

        [Fact]
        public void ConvertToEntity()
        {
            //Arrange
            var dto = _fixture.Build<SomeDto>().Create();

            //Act
            var entity = dto.ToEntity();

            //Assert
            var assignmentIssues = entity.AssignmentIssues().ToArray();
            assignmentIssues.Should().BeEmpty();
            entity.IsAssigned().Should().BeTrue();
        }
    }
}