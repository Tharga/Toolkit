using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Tharga.Toolkit.Tests;

public class UriExtensionsTests
{
    [Fact]
    public void RemoveQuery_strips_query_string()
    {
        var uri = new Uri("https://example.com/path?foo=bar&baz=1");
        var result = uri.RemoveQuery();
        result.Query.Should().BeEmpty();
        result.AbsolutePath.Should().Be("/path");
    }

    [Fact]
    public void RemoveQuery_returns_same_uri_when_no_query()
    {
        var uri = new Uri("https://example.com/path");
        var result = uri.RemoveQuery();
        result.Query.Should().BeEmpty();
        result.AbsolutePath.Should().Be("/path");
    }

    [Fact]
    public void GetQueryValue_returns_matching_value()
    {
        var uri = new Uri("https://example.com?name=hello");
        var result = uri.GetQueryValue("name").ToList();
        result.Should().ContainSingle().Which.Should().Be("hello");
    }

    [Fact]
    public void GetQueryValue_returns_multiple_values()
    {
        var uri = new Uri("https://example.com?tag=a&tag=b&tag=c");
        var result = uri.GetQueryValue("tag").ToList();
        result.Should().Equal("a", "b", "c");
    }

    [Fact]
    public void GetQueryValue_returns_empty_for_missing_name()
    {
        var uri = new Uri("https://example.com?foo=bar");
        uri.GetQueryValue("missing").Should().BeEmpty();
    }

    [Fact]
    public void GetQueryValue_returns_empty_for_no_query()
    {
        var uri = new Uri("https://example.com/path");
        uri.GetQueryValue("name").Should().BeEmpty();
    }

    [Fact]
    public void GetQueryValue_decodes_encoded_values()
    {
        var uri = new Uri("https://example.com?msg=hello%20world");
        uri.GetQueryValue("msg").First().Should().Be("hello world");
    }
}
