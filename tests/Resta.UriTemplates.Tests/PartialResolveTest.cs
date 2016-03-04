using Xunit;

namespace Resta.UriTemplates.Tests
{
    public class PartialResolveFacts
    {
        [Fact]
        public void PartialResolveSimpleFact()
        {
            var template = new UriTemplate("{host}{/path}{?query}{#fragment}");

            template = template.GetResolver().Bind("host", "example.com").ResolveTemplate();
            Assert.Equal("example.com{/path}{?query}{#fragment}", template.ToString());

            template = template.GetResolver().Bind("path", "path").ResolveTemplate();
            Assert.Equal("example.com/path{?query}{#fragment}", template.ToString());

            template = template.GetResolver().Bind("query", "value").ResolveTemplate();
            Assert.Equal("example.com/path?query=value{#fragment}", template.ToString());

            template = template.GetResolver().Bind("fragment", "fragment").ResolveTemplate();
            Assert.Equal("example.com/path?query=value#fragment", template.ToString());
        }

        [Fact]
        public void PartialResolveMultiplePathsFact()
        {
            var template = new UriTemplate("{/path1,path2,path3}");

            var partialTemplate = template.GetResolver().Bind("path1", "value").ResolveTemplate();
            Assert.Equal("/value{/path2,path3}", partialTemplate.ToString());

            partialTemplate = template.GetResolver().Bind("path2", "value").ResolveTemplate();
            Assert.Equal("{/path1}/value{/path3}", partialTemplate.ToString());

            partialTemplate = template.GetResolver().Bind("path3", "value").ResolveTemplate();
            Assert.Equal("{/path1,path2}/value", partialTemplate.ToString());
        }

        [Fact]
        public void PartialResolveMultipleContinuationsFact()
        {
            var template = new UriTemplate("{&name1,name2,name3}");

            var partialTemplate = template.GetResolver().Bind("name1", "value").ResolveTemplate();
            Assert.Equal("&name1=value{&name2,name3}", partialTemplate.ToString());

            partialTemplate = template.GetResolver().Bind("name2", "value").ResolveTemplate();
            Assert.Equal("{&name1}&name2=value{&name3}", partialTemplate.ToString());

            partialTemplate = template.GetResolver().Bind("name3", "value").ResolveTemplate();
            Assert.Equal("{&name1,name2}&name3=value", partialTemplate.ToString());
        }

        [Fact]
        public void PartialResolveMultipleQueryFact()
        {
            var template = new UriTemplate("{?param1,param2}");
            var actual = template.GetResolver().Bind("param1", "Fact").ResolveTemplate().ToString();
            Assert.Equal("?param1=Fact{&param2}", actual);
        }

        [Fact]
        public void PartialResolveMultipleQueryWithReorderingFact()
        {
            var template = new UriTemplate("{?param1,param2}");
            var actual = template.GetResolver().Bind("param2", "Fact").ResolveTemplate().ToString();
            Assert.Equal("?param2=Fact{&param1}", actual);
        }

        [Theory]
        [InlineData("#")]
        [InlineData("+")]
        [InlineData("")]
        public void PartialResolveAllVariablesFact(string exprOp)
        {
            var template = new UriTemplate("{" + exprOp + "param1,param2}");
            var partialTemplate =
                template.GetResolver().Bind("param1", "param1").Bind("param2", "param2").ResolveTemplate();
            var expected = (exprOp == "+" ? string.Empty : exprOp) + "param1,param2";
            Assert.Equal(expected, partialTemplate.ToString());
        }

        [Theory]
        [InlineData("#")]
        [InlineData("+")]
        [InlineData("")]
        public void PartialResolveNothingFact(string exprOp)
        {
            var template = new UriTemplate("{" + exprOp + "param1,param2}");
            var partialTemplate = template.GetResolver().Bind("unknown", "1").ResolveTemplate();

            Assert.Equal(template.ToString(), partialTemplate.ToString());
        }

        [Theory]
        [InlineData("#")]
        [InlineData("+")]
        [InlineData("")]
        public void ResolveIsNotAvailableForPartialVariablesFact(string exprOp)
        {
            var template = new UriTemplate("{" + exprOp + "param1,param2}");
            Assert.Throws<UriTemplateException>(() => template.GetResolver().Bind("param1", "Fact").ResolveTemplate());
            Assert.Throws<UriTemplateException>(() => template.GetResolver().Bind("param2", "Fact").ResolveTemplate());
        }
    }
}