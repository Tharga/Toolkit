using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Tharga.Toolkit.Compare;
using Tharga.Toolkit.Tests.Compare.Entities;
using Xunit;

namespace Tharga.Toolkit.Tests.Compare
{
    public class CompareTester
    {
        [Fact]
        public void When_comparing_a_date_with_itself()
        {
            // Arrange
            var o1 = new DateTime(2002, 3, 4, 5, 6, 7, 8);

            // Act
            var diffs = o1.Compare(o1).ToArray();

            // Assert
            diffs.Should().BeEmpty();
        }

        [Fact]
        public void When_comparing_two_identical_dates()
        {
            // Arrange
            var o1 = new DateTime(2002, 3, 4, 5, 6, 7, 8);
            var o2 = new DateTime(2002, 3, 4, 5, 6, 7, 8);

            // Act
            var diffs = o1.Compare(o2).ToArray();

            // Assert
            diffs.Should().BeEmpty();
        }

        [Fact]
        public void When_comparing_two_different_dates()
        {
            // Arrange
            var o1 = new DateTime(2002, 3, 4, 5, 6, 7, 8);
            var o2 = new DateTime(2002, 3, 4, 5, 55, 7, 8);

            // Act
            var diffs = o1.Compare(o2).ToArray();

            // Assert
            diffs.Should().HaveCount(1);
        }

        [Fact]
        public void When_comparing_an_object_with_null()
        {
            // Arrange
            var o1 = new SomeSimpleClass { Data = "A" };

            // Act
            var diffs = o1.Compare(null).ToArray();

            // Assert
            diffs.Should().HaveCount(1);
        }

        [Fact]
        public void When_comparing_null_with_an_object()
        {
            // Arrange
            var o1 = (SomeSimpleClass)null;
            var o2 = new SomeSimpleClass { Data = "A" };

            // Act
            var diffs = o1.Compare(o2).ToArray();

            // Assert
            diffs.Should().HaveCount(1);
        }

        [Fact]
        public void When_comparing_null_with_null()
        {
            // Arrange
            var o1 = (SomeSimpleClass)null;
            var o2 = (SomeSimpleClass)null;

            // Act
            var diffs = o1.Compare(o2).ToArray();

            // Assert
            diffs.Should().BeEmpty();
        }

        [Fact]
        public void When_comparing_null_with_a_string()
        {
            // Arrange
            var o1 = (string)null;
            var o2 = "ABC";

            // Act
            var diffs = o1.Compare(o2).ToArray();

            // Assert
            diffs.Should().HaveCount(1);
        }

        [Fact]
        public void When_comparing_two_identical_lists_where_the_content_has_nulls()
        {
            // Arrange
            var o1 = new List<string> { "A", null, "C" };
            var o2 = new List<string> { "A", null, "C" };

            // Act
            var diffs = o1.Compare(o2).ToArray();

            // Assert
            diffs.Should().BeEmpty();
        }

        [Fact]
        public void When_comparing_two_differennt_lists_where_the_content_has_nulls()
        {
            // Arrange
            var o1 = new List<string> { "A", null, "C" };
            var o2 = new List<string> { "A", string.Empty, "C" };

            // Act
            var diffs = o1.Compare(o2).ToArray();

            // Assert
            diffs.Should().HaveCount(1);
        }

        [Fact]
        public void When_comparing_two_different_types_and_both_are_null()
        {
            // Arrange
            var o1 = (string)null;
            var o2 = (SomeSimpleClass)null;

            // Act
            var diffs = o1.Compare(o2).ToArray();

            // Assert
            diffs.Should().BeEmpty();
        }

        [Fact]
        public void When_comparing_objects_that_are_linked_in_a_circle()
        {
            // Arrange
            var obj1 = new SomeCircleClass { Data = "ABC" };
            var obj2 = new SomeCircleClass { Data = "ABC", Ref = obj1 };
            var obj3 = new SomeCircleClass { Data = "ABC", Ref = obj2 };
            var obj4 = new SomeCircleClass { Data = "ABC", Ref = obj3 };
            obj1.Ref = obj4;

            // Act
            var diffs = obj1.Compare(obj2);

            // Assert
            diffs.Should().BeEmpty();
        }

        [Fact]
        public void When_comparing_two_arrays_where_one_is_longer_than_the_other()
        {
            // Arrange
            var d1 = new string[] { "A", "B", "C" };
            var d2 = new string[] { "A", "B" };

            // Act
            var diffs = d1.Compare(d2).ToArray();

            // Assert
            diffs.Should().HaveCount(1);
        }

        [Fact]
        public void When_comparing_two_arrays_where_one_is_shorter_than_the_other()
        {
            // Arrange
            var d1 = new string[] { "A", "B" };
            var d2 = new string[] { "A", "B", "C" };

            // Act
            var diffs = d1.Compare(d2).ToArray();

            // Assert
            diffs.Should().HaveCount(1);
        }

        [Fact]
        public void When_comparing_two_arrays_with_same_length_but_different_values()
        {
            // Arrange
            var d1 = new string[] { "A", "B", "C" };
            var d2 = new string[] { "A", "BB", "C" };

            // Act
            var diffs = d1.Compare(d2).ToArray();

            // Assert
            diffs.Should().HaveCount(1);
            diffs.First().Message.Should().Be("One string has value B and the other string has value BB.");
            diffs.First().ObjectName.Should().Be("String[1].String");
            diffs.First().Index.Should().Be(1);
        }

        //ARR

        [Fact]
        public void When_comparing_a_dictionary_with_itself()
        {
            // Arrange
            var d = new Dictionary<int, string> { { 1, "A" }, { 2, "B" }, { 3, "C" } };

            // Act
            var diffs = d.Compare(d).ToArray();

            // Assert
            diffs.Should().BeEmpty();
        }

        [Fact]
        public void When_comparing_two_identical_dictionaries_with_eachother()
        {
            // Arrange
            var d1 = new Dictionary<int, string> { { 1, "A" }, { 2, "B" }, { 3, "C" } };
            var d2 = new Dictionary<int, string> { { 1, "A" }, { 2, "B" }, { 3, "C" } };

            // Act
            var diffs = d1.Compare(d2).ToArray();

            // Assert
            diffs.Should().BeEmpty();
        }

        [Fact]
        public void When_comparing_two_dictionaries_where_one_is_longer_than_the_other()
        {
            // Arrange
            var d1 = new Dictionary<int, string> { { 1, "A" }, { 2, "B" }, { 3, "C" } };
            var d2 = new Dictionary<int, string> { { 1, "A" }, { 2, "B" } };

            // Act
            var diffs = d1.Compare(d2).ToArray();

            // Assert
            diffs.Should().HaveCount(1);
        }

        [Fact]
        public void When_comparing_two_dictionaries_where_one_is_shorter_than_the_other()
        {
            // Arrange
            var d1 = new Dictionary<int, string> { { 1, "A" }, { 2, "B" } };
            var d2 = new Dictionary<int, string> { { 1, "A" }, { 2, "B" }, { 3, "C" } };

            // Act
            var diffs = d1.Compare(d2).ToArray();

            // Assert
            diffs.Should().HaveCount(1);
        }

        [Fact]
        public void When_comparing_two_dictionaries_with_same_length_but_different_values()
        {
            // Arrange
            var d1 = new Dictionary<int, string> { { 1, "A" }, { 2, "B" }, { 3, "C" } };
            var d2 = new Dictionary<int, string> { { 1, "A" }, { 2, "BB" }, { 3, "C" } };

            // Act
            var diffs = d1.Compare(d2).ToArray();

            // Assert
            diffs.Should().HaveCount(1);
        }

        [Fact]
        public void When_comparing_two_dictionaries_with_same_length_but_different_keys()
        {
            // Arrange
            var d1 = new Dictionary<int, string> { { 1, "A" }, { 2, "B" }, { 3, "C" } };
            var d2 = new Dictionary<int, string> { { 1, "A" }, { 22, "B" }, { 3, "C" } };

            // Act
            var diffs = d1.Compare(d2).ToArray();

            // Assert
            diffs.Should().HaveCount(1);
        }

        [Fact]
        public void When_comparing_two_identical_list_with_objects_with_eachother()
        {
            // Arrange
            var list1 = new List<SomeSimpleClass> { new SomeSimpleClass { Data = "A" }, new SomeSimpleClass { Data = "B" }, new SomeSimpleClass { Data = "C" } };
            var list2 = new List<SomeSimpleClass> { new SomeSimpleClass { Data = "A" }, new SomeSimpleClass { Data = "B" }, new SomeSimpleClass { Data = "C" } };

            // Act
            var diffs = list1.Compare(list2).ToArray();

            // Assert
            diffs.Should().BeEmpty();
        }

        [Fact]
        public void When_comparing_two_list_with_objects_where_one_list_is_longer_than_the_other()
        {
            // Arrange
            var list1 = new List<SomeSimpleClass> { new SomeSimpleClass { Data = "A" }, new SomeSimpleClass { Data = "B" }, new SomeSimpleClass { Data = "C" } };
            var list2 = new List<SomeSimpleClass> { new SomeSimpleClass { Data = "A" }, new SomeSimpleClass { Data = "B" } };

            // Act
            var diffs = list1.Compare(list2).ToArray();

            // Assert
            diffs.Should().HaveCount(1);
        }

        [Fact]
        public void When_comparing_two_list_with_objects_where_one_list_is_shorter_than_the_other()
        {
            // Arrange
            var list1 = new List<SomeSimpleClass> { new SomeSimpleClass { Data = "A" }, new SomeSimpleClass { Data = "B" } };
            var list2 = new List<SomeSimpleClass> { new SomeSimpleClass { Data = "A" }, new SomeSimpleClass { Data = "B" }, new SomeSimpleClass { Data = "C" } };

            // Act
            var diffs = list1.Compare(list2).ToArray();

            // Assert
            diffs.Should().HaveCount(1);
        }

        [Fact]
        public void When_comparing_two_list_with_objects_that_differs_in_content()
        {
            // Arrange
            var list1 = new List<SomeSimpleClass> { new SomeSimpleClass { Data = "A" }, new SomeSimpleClass { Data = "B" }, new SomeSimpleClass { Data = "C" } };
            var list2 = new List<SomeSimpleClass> { new SomeSimpleClass { Data = "A" }, new SomeSimpleClass { Data = "BB" }, new SomeSimpleClass { Data = "C" } };

            // Act
            var diffs = list1.Compare(list2).ToArray();

            // Assert
            diffs.Should().HaveCount(1);
        }

        [Fact]
        public void When_comparing_a_list_with_itself()
        {
            // Arrange
            var list1 = new List<string> { "A", "B", "C" };

            // Act
            var diffs = list1.Compare(list1).ToArray();

            // Assert
            diffs.Should().BeEmpty();
        }

        [Fact]
        public void When_comparing_two_identical_list_with_eachother()
        {
            // Arrange
            var list1 = new List<string> { "A", "B", "C" };
            var list2 = new List<string> { "A", "B", "C" };

            // Act
            var diffs = list1.Compare(list2).ToArray();

            // Assert
            diffs.Should().BeEmpty();
        }

        [Fact]
        public void When_comparing_two_list_where_one_is_longer_than_the_other()
        {
            // Arrange
            var list1 = new List<string> { "A", "B", "C" };
            var list2 = new List<string> { "A", "B" };

            // Act
            var diffs = list1.Compare(list2).ToArray();

            // Assert
            diffs.Should().HaveCount(1);
        }

        [Fact]
        public void When_comparing_two_list_where_one_is_shorter_than_the_other()
        {
            // Arrange
            var list1 = new List<string> { "A", "B" };
            var list2 = new List<string> { "A", "B", "C" };

            // Act
            var diffs = list1.Compare(list2).ToArray();

            // Assert
            diffs.Should().HaveCount(1);
        }


        [Fact]
        public void When_comparing_two_list_with_same_length_but_different_content()
        {
            // Arrange
            var list1 = new List<string> { "A", "B", "C" };
            var list2 = new List<string> { "A", "BB", "C" };

            // Act
            var diffs = list1.Compare(list2).ToArray();

            // Assert
            diffs.Should().HaveCount(1);
        }

        [Fact]
        public void When_comparing_two_identical_guids()
        {
            // Arrange
            var d1 = Guid.NewGuid();
            var d2 = new Guid(d1.ToString());

            // Act
            var diffs = d1.Compare(d2).ToArray();

            // Assert
            diffs.Any().Should().BeFalse();
            diffs.Should().BeEmpty();
        }

        [Fact]
        public void When_comparing_two_different_guids()
        {
            // Arrange
            var d1 = Guid.NewGuid();
            var d2 = Guid.NewGuid();

            // Act
            var diffs = d1.Compare(d2).ToArray();

            // Assert
            diffs.Should().HaveCount(1);
        }

        [Fact]
        public void When_comparing_two_complex_objects_that_differs()
        {
            // Arrange
            var tc1 = new SomeTestClass(77) { ClassListProperty = new List<SomeTestClass> { new SomeTestClass(7) }, StringProperty = "A", Dictionary = new Dictionary<string, SomeTestClass> { { "A", new SomeTestClass(1) } }, StringListMember = new List<string> { "A", "B", "C" }, StringMember = "B" };
            var tc2 = new SomeTestClass(88) { ClassListProperty = new List<SomeTestClass> { new SomeTestClass(8) }, StringProperty = "AA", Dictionary = new Dictionary<string, SomeTestClass> { { "AA", new SomeTestClass(2) } }, StringListMember = new List<string> { "A", "BB", "C" }, StringMember = "BB" };

            // Act
            var diffs = tc1.Compare(tc2).ToArray();

            // Assert
            diffs.Should().HaveCount(7);
        }

        [Fact]
        public void When_comparing_two_simple_classes_with_members()
        {
            // Arrange
            var tc1 = new SomeOtherSimpleClass { Data = "A" };
            var tc2 = new SomeOtherSimpleClass { Data = "B" };

            // Act
            var diffs = tc1.Compare(tc2).ToArray();

            // Assert
            diffs.Should().HaveCount(1);
        }

        [Fact]
        public void When_comparing_two_complex_and_identical_objects_with_eachother()
        {
            // Arrange
            var tc1 = new SomeTestClass(88);
            var tc2 = new SomeTestClass(88);

            // Act
            var diffs = tc1.Compare(tc2).ToArray();

            // Assert
            diffs.Any().Should().BeFalse();
            diffs.Should().BeEmpty();
        }

        [Fact]
        public void When_comparing_one_complex_struct_with_itself()
        {
            // Arrange
            var tc1 = new SomeTestStruct { StringProperty = "A", StructListProperty = new List<SomeTestStruct>(), StringListMember = new List<string> { "B" }, StringMember = "C" };

            // Act
            var diffs = tc1.Compare(tc1).ToArray();

            // Assert
            diffs.Should().BeEmpty();
        }

        [Fact]
        public void When_comparing_two_complex_and_different_structs_with_eachother()
        {
            // Arrange
            var tc1 = new SomeTestStruct { StringProperty = "A", StructListProperty = new List<SomeTestStruct>(), StringListMember = new List<string> { "B" }, StringMember = "C" };
            var tc2 = new SomeTestStruct { StringProperty = "AA", StructListProperty = new List<SomeTestStruct>(), StringListMember = new List<string> { "BB" }, StringMember = "CC" };

            // Act
            var diffs = tc1.Compare(tc2).ToArray();

            // Assert
            diffs.Should().HaveCount(3);
        }

        [Fact]
        public void When_comparing_two_complex_and_identical_structs_with_eachother()
        {
            // Arrange
            var tc1 = new SomeTestStruct { StringProperty = "A", StructListProperty = new List<SomeTestStruct>(), StringListMember = new List<string> { "B" }, StringMember = "C" };
            var tc2 = new SomeTestStruct { StringProperty = "A", StructListProperty = new List<SomeTestStruct>(), StringListMember = new List<string> { "B" }, StringMember = "C" };

            // Act
            var diffs = tc1.Compare(tc2).ToArray();

            // Assert
            diffs.Any().Should().BeFalse();
            diffs.Should().BeEmpty();
        }

        [Fact]
        public void When_comparing_two_objects_with_several_differences()
        {
            // Arrange
            var o1 = new SomeClass { Data1 = "A", Data2 = 1 };
            var o2 = new SomeClass { Data1 = "B", Data2 = 2 };

            // Act
            var diffs = o1.Compare(o2).ToArray();

            // Assert
            diffs.Should().HaveCount(2);
        }

        [Fact]
        public void When_comparing_the_same_object_with_itself()
        {
            // Arrange
            var tc = new object();

            // Act
            var diffs = tc.Compare(tc).ToArray();

            // Assert
            diffs.Any().Should().BeFalse();
            diffs.Should().BeEmpty();
        }

        [Fact]
        public void When_comparing_two_simple_identical_objects()
        {
            // Arrange
            var o1 = new SomeSimpleClass { Data = "A" };
            var o2 = new SomeSimpleClass { Data = "A" };

            // Act
            var diffs = o1.Compare(o2).ToArray();

            // Assert
            diffs.Any().Should().BeFalse();
            diffs.Should().BeEmpty();
        }

        [Fact]
        public void When_comparing_two_simple_objects_that_differs()
        {
            // Arrange
            var o1 = new SomeSimpleClass { Data = "A" };
            var o2 = new SomeSimpleClass { Data = "B" };

            // Act
            var diffs = o1.Compare(o2).ToArray();

            // Assert
            diffs.Should().HaveCount(1);
        }

        [Fact]
        public void When_comparing_strings_that_are_equal()
        {
            // Arrange
            var s1 = "ABC";
            var s2 = "ABC";

            // Act
            var diffs = s1.Compare(s2).ToArray();

            // Assert
            diffs.Any().Should().BeFalse();
            diffs.Should().BeEmpty();
        }

        [Fact]
        public void When_comparing_strings_that_are_not_equal()
        {
            // Arrange
            var s1 = "ABC1";
            var s2 = "ABC2";

            // Act
            var response = s1.Compare(s2).ToArray();

            // Assert
            response.Any().Should().BeTrue();
            response.Should().HaveCount(1);
        }

        [Fact]
        public void When_comparing_ints_that_are_equal()
        {
            // Arrange
            var i1 = 123;
            var i2 = 123;

            // Act
            var diffs = i1.Compare(i2).ToArray();

            // Assert
            diffs.Any().Should().BeFalse();
        }

        [Fact]
        public void When_comparing_decimals_that_are_equal()
        {
            // Arrange
            var d1 = 123.4M;
            var d2 = 123.4M;

            // Act
            var diffs = d1.Compare(d2).ToArray();

            // Assert
            diffs.Any().Should().BeFalse();
        }

        [Fact]
        public void When_comparing_ints_that_are_not_equal()
        {
            // Arrange
            var i1 = 123;
            var i2 = 124;

            // Act
            var diffs = i1.Compare(i2).ToArray();

            // Assert
            diffs.Any().Should().BeTrue();
        }

        [Fact]
        public void When_comparing_an_int_and_a_string_with_the_same_content()
        {
            // Arrange
            var d1 = 123;
            var d2 = "123";

            // Act
            var diffs = d1.Compare(d2).ToArray();

            // Assert
            diffs.Any().Should().BeTrue();
        }

        [Fact]
        public void When_comparing_two_doubles_that_are_equal()
        {
            // Arrange
            var d1 = 123.5;
            var d2 = 123.5;

            // Act
            var diffs = d1.Compare(d2).ToArray();

            // Assert
            diffs.Any().Should().BeFalse();
        }

        [Fact]
        public void When_comparing_two_decimals_that_are_not_equal()
        {
            // Arrange
            var d1 = (decimal)123.50;
            var d2 = (decimal)123.56;

            // Act
            var diffs = d1.Compare(d2).ToArray();

            // Assert
            diffs.Any().Should().BeTrue();
        }

        [Fact]
        public void When_comparing_two_different_classes_with_the_same_content_and_the_type_is_ignored()
        {
            // Arrange
            var v1 = new SomeSimpleClass { Data = "ABC" };
            var v2 = new OtherSomeSimpleClass { Data = "ABC" };

            // Act
            var diffs = v1.Compare(v2, CompareExtensions.CompareMode.IgnoreType).ToArray();

            // Assert
            diffs.Should().BeEmpty();
        }

        [Fact]
        public void When_comparing_two_different_classes_with_the_same_content_but_different_types_and_the_type_is_ignored()
        {
            // Arrange
            var v1 = new SomeSimpleClass { Data = "ABC" };
            var v2 = new SomeOtherSimpleClass { Data = "ABC" };

            // Act
            var diffs = v1.Compare(v2, CompareExtensions.CompareMode.IgnoreType).ToArray();

            // Assert
            diffs.Should().BeEmpty();
        }

        [Fact]
        public void When_comparing_two_different_classes_with_the_same_content_but_different_backwords_types_and_the_type_is_ignored()
        {
            // Arrange
            var v1 = new SomeSimpleClass { Data = "ABC" };
            var v2 = new SomeOtherSimpleClass { Data = "ABC" };

            // Act
            var diffs = v2.Compare(v1, CompareExtensions.CompareMode.IgnoreType).ToArray();

            // Assert
            diffs.Should().BeEmpty();
        }

        [Fact]
        public void When_comparing_two_different_classes_with_the_same_content_and_the_type_is_not_ignored()
        {
            // Arrange
            var v1 = new SomeSimpleClass { Data = "ABC" };
            var v2 = new OtherSomeSimpleClass { Data = "ABC" };

            // Act
            var diffs = v1.Compare(v2, CompareExtensions.CompareMode.Standard).ToArray();

            // Assert
            diffs.Should().HaveCount(1);
        }

        [Fact]
        public void When_comparing_two_different_classes_with_the_same_content_but_different_types_and_the_type_is_not_ignored()
        {
            // Arrange
            var v1 = new SomeSimpleClass { Data = "ABC" };
            var v2 = new SomeOtherSimpleClass { Data = "ABC" };

            // Act
            var diffs = v1.Compare(v2, CompareExtensions.CompareMode.Standard).ToArray();

            // Assert
            diffs.Should().HaveCount(1);
        }

        [Fact]
        public void When_comparing_an_int_with_a_string_with_the_same_value_and_the_type_is_ignored()
        {
            // Arrange
            var v1 = "123";
            var v2 = 123;

            // Act
            var diffs = v1.Compare(v2, CompareExtensions.CompareMode.IgnoreType).ToArray();

            // Assert
            diffs.Should().BeEmpty();
        }

        [Fact]
        public void When_comparing_a_string_with_an_int_with_the_same_value_and_the_type_is_ignored()
        {
            // Arrange
            var v1 = "123";
            var v2 = 123;

            // Act
            var diffs = v2.Compare(v1, CompareExtensions.CompareMode.IgnoreType).ToArray();

            // Assert
            diffs.Should().BeEmpty();
        }

        [Fact]
        public void When_comparing_an_int_and_a_string_with_different_value_and_the_type_is_ignored()
        {
            // Arrange
            var v1 = "123";
            var v2 = 1234;

            // Act
            var diffs = v1.Compare(v2, CompareExtensions.CompareMode.IgnoreType).ToArray();

            // Assert
            diffs.Should().HaveCount(1);
        }

        [Fact]
        public void When_comparing_a_double_with_a_decimal_and_the_type_is_ignored()
        {
            // Arrange
            var v1 = (double)123;
            var v2 = (decimal)123;

            // Act
            var diffs = v1.Compare(v2, CompareExtensions.CompareMode.IgnoreType).ToArray();

            // Assert
            diffs.Should().BeEmpty();
        }

        [Fact]
        public void When_comparing_an_int_with_a_double_and_the_type_is_ignored()
        {
            // Arrange
            var v1 = 123;
            var v2 = (double)123;

            // Act
            var diffs = v1.Compare(v2, CompareExtensions.CompareMode.IgnoreType).ToArray();

            // Assert
            diffs.Should().BeEmpty();
        }

        [Fact]
        public void When_comparing_an_int_with_a_double_with_decimals_and_the_type_is_ignored()
        {
            // Arrange
            var v1 = 123;
            var v2 = 123.1;

            // Act
            var diffs = v1.Compare(v2, CompareExtensions.CompareMode.IgnoreType).ToArray();

            // Assert
            diffs.Should().HaveCount(1);
        }

        [Fact]
        public void When_comparing_a_double_with_decimal_with_an_int_and_the_type_is_ignored()
        {
            // Arrange
            var v1 = 123.1;
            var v2 = 123;

            // Act
            var diffs = v1.Compare(v2, CompareExtensions.CompareMode.IgnoreType).ToArray();

            // Assert
            diffs.Should().HaveCount(1);
        }

        [Fact]
        public void When_comparing_lists_that_are_ordered_differently()
        {
            // Arrange
            var o1 = new List<string> { "A", "B", "C" };
            var o2 = new List<string> { "A", "C", "B" };

            // Act
            var diffs = o1.Compare(o2).ToArray();

            // Assert
            diffs.Should().HaveCount(2);
        }

        [Fact]
        public void When_comparing_lists_that_are_ordered_differently_and_sort_order_is_ignored()
        {
            // Arrange
            var o1 = new List<string> { "A", "B", "C" };
            var o2 = new List<string> { "A", "C", "B" };

            // Act
            var diffs = o1.Compare(o2, CompareExtensions.CompareMode.IgnoreSortOrder).ToArray();

            // Assert
            diffs.Should().BeEmpty();
        }

        [Fact]
        public void When_comparing_lists_that_are_ordered_differently_and_sort_order_is_ignored_()
        {
            // Arrange
            var o1 = new List<string> { "A", "B", "B" };
            var o2 = new List<string> { "A", "C", "B" };

            // Act
            var diffs = o1.Compare(o2, CompareExtensions.CompareMode.IgnoreSortOrder).ToArray();

            // Assert
            diffs.Should().HaveCount(1);
        }

        [Fact]
        public void When_comparing_lists_that_are_ordered_differently_and_sort_order_is_ignored_2()
        {
            // Arrange
            var o1 = new List<string> { "A", "B", "C" };
            var o2 = new List<string> { "A", "C", "C" };

            // Act
            var diffs = o1.Compare(o2, CompareExtensions.CompareMode.IgnoreSortOrder).ToArray();

            // Assert
            diffs.Should().HaveCount(1);
        }

        [Fact]
        public void When_comparing_lists_with_complex_objects_that_are_ordered_differently()
        {
            // Arrange
            var o1 = new List<SomeSimpleClass> { new SomeSimpleClass { Data = "A" }, new SomeSimpleClass { Data = "B" }, new SomeSimpleClass { Data = "C" } };
            var o2 = new List<SomeSimpleClass> { new SomeSimpleClass { Data = "A" }, new SomeSimpleClass { Data = "C" }, new SomeSimpleClass { Data = "B" } };

            // Act
            var diffs = o1.Compare(o2).ToArray();

            // Assert
            diffs.Should().HaveCount(2);
        }

        [Fact]
        public void When_comparing_lists_with_complex_objects_that_are_ordered_differently_and_sort_order_is_ignored()
        {
            // Arrange
            var o1 = new List<SomeSimpleClass> { new SomeSimpleClass { Data = "A" }, new SomeSimpleClass { Data = "B" }, new SomeSimpleClass { Data = "C" } };
            var o2 = new List<SomeSimpleClass> { new SomeSimpleClass { Data = "A" }, new SomeSimpleClass { Data = "C" }, new SomeSimpleClass { Data = "B" } };

            // Act
            var diffs = o1.Compare(o2, CompareExtensions.CompareMode.IgnoreSortOrder).ToArray();

            // Assert
            diffs.Should().BeEmpty();
        }

        [Fact]
        public void When_comparing_lists_with_complex_objects_that_are_ordered_differently_and_sort_order_is_ignored_()
        {
            // Arrange
            var o1 = new List<SomeSimpleClass> { new SomeSimpleClass { Data = "A" }, new SomeSimpleClass { Data = "B" }, new SomeSimpleClass { Data = "B" } };
            var o2 = new List<SomeSimpleClass> { new SomeSimpleClass { Data = "A" }, new SomeSimpleClass { Data = "C" }, new SomeSimpleClass { Data = "B" } };

            // Act
            var diffs = o1.Compare(o2, CompareExtensions.CompareMode.IgnoreSortOrder).ToArray();

            // Assert
            diffs.Should().HaveCount(1);
        }

        [Fact]
        public void When_comparing_lists_with_complex_objects_that_are_ordered_differently_and_sort_order_is_ignored_2()
        {
            // Arrange
            var o1 = new List<SomeSimpleClass> { new SomeSimpleClass { Data = "A" }, new SomeSimpleClass { Data = "B" }, new SomeSimpleClass { Data = "C" } };
            var o2 = new List<SomeSimpleClass> { new SomeSimpleClass { Data = "A" }, new SomeSimpleClass { Data = "C" }, new SomeSimpleClass { Data = "C" } };

            // Act
            var diffs = o1.Compare(o2, CompareExtensions.CompareMode.IgnoreSortOrder).ToArray();

            // Assert
            diffs.Should().HaveCount(1);
        }

        [Fact]
        public void When_comparing_lists_with_complex_objects_of_different_types_that_are_ordered_differently_and_sort_order_is_ignored()
        {
            // Arrange
            var o1 = new List<SomeSimpleClass> { new SomeSimpleClass { Data = "A" }, new SomeSimpleClass { Data = "B" }, new SomeSimpleClass { Data = "C" } };
            var o2 = new List<SomeOtherSimpleClass> { new SomeOtherSimpleClass { Data = "A" }, new SomeOtherSimpleClass { Data = "C" }, new SomeOtherSimpleClass { Data = "B" } };

            // Act
            var diffs = o1.Compare(o2, CompareExtensions.CompareMode.IgnoreSortOrder).ToArray();

            // Assert
            diffs.Should().HaveCount(1);
        }

        [Fact]
        public void When_comparing_lists_with_complex_objects_of_different_types_that_are_ordered_differently_and_sort_order_and_type_is_ignored()
        {
            // Arrange
            var o1 = new List<SomeSimpleClass> { new SomeSimpleClass { Data = "A" }, new SomeSimpleClass { Data = "B" }, new SomeSimpleClass { Data = "C" } };
            var o2 = new List<SomeOtherSimpleClass> { new SomeOtherSimpleClass { Data = "A" }, new SomeOtherSimpleClass { Data = "C" }, new SomeOtherSimpleClass { Data = "B" } };

            // Act
            var diffs = o1.Compare(o2, CompareExtensions.CompareMode.IgnoreType | CompareExtensions.CompareMode.IgnoreSortOrder).ToArray();

            // Assert
            diffs.Should().BeEmpty();
        }
    }
}