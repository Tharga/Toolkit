using System.Linq;
using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;
using FluentAssertions;
using Tharga.Test.Toolkit;
using Tharga.Toolkit.Tests.Assignment.Entities;
using Xunit;

namespace Tharga.Toolkit.Tests.Assignment
{
    public class AssignmentClassTester
    {
        private readonly Fixture _fixture;

        public AssignmentClassTester()
        {
            _fixture = new Fixture();
            _fixture.Register(() => ESomeEnum.B);
            _fixture.Register(() => true);
        }

        [Fact]
        public void StaticConverter()
        {
            //Arrange
            var type = typeof(Converter);
            var ignoreProperties = new string[] {};
            var ignoreFunctions = new[] {"ToBrokenDto"};
            _fixture.Register(() => true);

            //Act
            var responses = type.TestAssignments(x => new SpecimenContext(_fixture).Resolve(x), ignoreFunctions).ToArray();

            //Assert
            foreach (var response in responses)
            {
                var assignmentIssues = response.data.AssignmentIssues(ignoreProperties).ToArray();
                assignmentIssues.Should().BeEmpty($"method '{response.method}' failed.");
                response.data.IsAssigned(ignoreProperties).Should().BeTrue();
            }
        }

        [Fact]
        public void ConverterClass()
        {
            //Arrange
            var converter = new ConverterInstance();
            var ignoreProperties = new string[] { };
            var ignoreFunctions = new[] { "ToBrokenDto", "Finalize" };
            _fixture.Register(() => true);

            //Act
            var responses = converter.TestAssignments(x => new SpecimenContext(_fixture).Resolve(x), ignoreFunctions, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).ToArray();

            //Assert
            foreach (var response in responses)
            {
                var assignmentIssues = response.data.AssignmentIssues(ignoreProperties).ToArray();
                assignmentIssues.Should().BeEmpty($"method '{response.method}' failed.");
                response.data.IsAssigned(ignoreProperties).Should().BeTrue();
            }
        }
    }
}