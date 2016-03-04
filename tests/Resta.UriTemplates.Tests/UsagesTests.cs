using System.Collections.Generic;
using Xunit;

namespace Resta.UriTemplates.Tests
{
    public class UsagesTests
    {
        [Fact]
        public void PathSegmentTest()
        {
            var template = new UriTemplate("http://example.com/{path}");

            var actual = template.Resolve(new Dictionary<string, object>
            {
                {"path", "foo"}
            });

            Assert.Equal("http://example.com/foo", actual);
        }

        [Fact]
        public void MultiplePathSegmentTest()
        {
            var template = new UriTemplate("http://example.com/{path1}/{path2}");

            var actual = template.Resolve(new Dictionary<string, object>
            {
                {"path1", "foo"},
                {"path2", "bar"}
            });

            Assert.Equal("http://example.com/foo/bar", actual);
        }

        [Fact]
        public void QueryParamsWithoutValuesTest()
        {
            var template = new UriTemplate("http://example.com/foo{?q1,q2}");

            var actual = template.Resolve(new Dictionary<string, object>());

            Assert.Equal("http://example.com/foo", actual);
        }

        [Fact]
        public void QueryParamsWithOneValueTest()
        {
            var template = new UriTemplate("http://example.com/foo{?q1,q2}");

            var actual = template.Resolve(new Dictionary<string, object>
            {
                {"q1", "abc"}
            });

            Assert.Equal("http://example.com/foo?q1=abc", actual);
        }

        [Fact]
        public void QueryParamsTest()
        {
            var template = new UriTemplate("http://example.com/foo{?q1,q2}");

            var actual = template.Resolve(new Dictionary<string, object>
            {
                {"q1", "abc"},
                {"q2", "10"}
            });

            Assert.Equal("http://example.com/foo?q1=abc&q2=10", actual);
        }

        [Fact]
        public void QueryParamsWithMultipleValuesTest()
        {
            var template = new UriTemplate("http://example.com/foo{?q1,q2}");

            var actual = template.Resolve(new Dictionary<string, object>
            {
                {"q1", new[] {"abc", "def", "ghi"}},
                {"q2", "10"}
            });

            Assert.Equal("http://example.com/foo?q1=abc,def,ghi&q2=10", actual);
        }

        [Fact]
        public void QueryParamsExpandedTest()
        {
            var template = new UriTemplate("http://example.com/foo{?q*}");

            var actual = template.Resolve(new Dictionary<string, object>
            {
                {"q", new Dictionary<string, string> {{"q1", "abc"}, {"q2", "def"}}}
            });

            Assert.Equal("http://example.com/foo?q1=abc&q2=def", actual);
        }

        [Fact]
        public void ExampleTest()
        {
            var template = new UriTemplate("http://example.org/{area}/last-news{?type,count}");

            var uri = template.Resolve(new Dictionary<string, object>
            {
                {"area", "world"},
                {"type", "actual"},
                {"count", "10"}
            });

            Assert.Equal("http://example.org/world/last-news?type=actual&count=10", uri);
        }

        [Fact]
        public void AnotherExampleTest()
        {
            var template = new UriTemplate("http://example.org/{area}/last-news{?type}");

            var uri = template.GetResolver()
                .Bind("area", "world")
                .Bind("type", new[] {"it", "music", "art"})
                .Resolve();

            Assert.Equal("http://example.org/world/last-news?type=it,music,art", uri);
        }

        [Fact]
        public void ComplexTest()
        {
            var template = new UriTemplate("http://example.com{/paths*}{?q1,q2}{#f*}");

            var actual = template.Resolve(new Dictionary<string, object>
            {
                {"paths", new[] {"foo", "bar"}},
                {"q1", "abc"},
                {"f", new Dictionary<string, string> {{"key1", "val1"}, {"key2", null}}}
            });

            Assert.Equal("http://example.com/foo/bar?q1=abc#key1=val1,key2=", actual);
        }

        [Fact]
        public void InvalidUriTemplateTest()
        {
            var template = "http://example.com/{path#!!}";

            try
            {
                new UriTemplate(template);
            }
            catch (UriTemplateParseException ex)
            {
                Assert.Equal(ex.Position, 24); // invalid variable name
                Assert.Equal(ex.Template, template);
                return;
            }

            Assert.Empty("Test must die");
        }
    }
}