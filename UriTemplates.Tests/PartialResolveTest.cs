namespace UriTemplates.Tests
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using Resta.UriTemplates;

    [TestFixture]
    public class PartialResolveTests
    {
        [Test]
        public void PartialResolveSimpleTest()
        {
            var template = new UriTemplate("{host}{/path}{?query}{#fragment}");

            template = template.GetResolver().Bind("host", "example.com").ResolveTemplate();
            Assert.AreEqual("example.com{/path}{?query}{#fragment}", template.ToString());

            template = template.GetResolver().Bind("path", "path").ResolveTemplate();
            Assert.AreEqual("example.com/path{?query}{#fragment}", template.ToString());

            template = template.GetResolver().Bind("query", "value").ResolveTemplate();
            Assert.AreEqual("example.com/path?query=value{#fragment}", template.ToString());

            template = template.GetResolver().Bind("fragment", "fragment").ResolveTemplate();
            Assert.AreEqual("example.com/path?query=value#fragment", template.ToString());
        }

        [Test]
        public void PartialResolveMultiplePathsTest()
        {
            UriTemplate partialTemplate;
            var template = new UriTemplate("{/path1,path2,path3}");

            partialTemplate = template.GetResolver().Bind("path1", "value").ResolveTemplate();
            Assert.AreEqual("/value{/path2,path3}", partialTemplate.ToString());

            partialTemplate = template.GetResolver().Bind("path2", "value").ResolveTemplate();
            Assert.AreEqual("{/path1}/value{/path3}", partialTemplate.ToString());

            partialTemplate = template.GetResolver().Bind("path3", "value").ResolveTemplate();
            Assert.AreEqual("{/path1,path2}/value", partialTemplate.ToString());
        }

        [Test]
        public void PartialResolveMultipleContinuationsTest()
        {
            UriTemplate partialTemplate;
            var template = new UriTemplate("{&name1,name2,name3}");

            partialTemplate = template.GetResolver().Bind("name1", "value").ResolveTemplate();
            Assert.AreEqual("&name1=value{&name2,name3}", partialTemplate.ToString());

            partialTemplate = template.GetResolver().Bind("name2", "value").ResolveTemplate();
            Assert.AreEqual("{&name1}&name2=value{&name3}", partialTemplate.ToString());

            partialTemplate = template.GetResolver().Bind("name3", "value").ResolveTemplate();
            Assert.AreEqual("{&name1,name2}&name3=value", partialTemplate.ToString());
        }

        [Test]
        public void PartialResolveMultipleQueryTest()
        {
            var template = new UriTemplate("{?param1,param2}");
            var actual = template.GetResolver().Bind("param1", "test").ResolveTemplate().ToString();
            Assert.AreEqual("?param1=test{&param2}", actual);
        }

        [Test]
        public void PartialResolveMultipleQueryWithReorderingTest()
        {
            var template = new UriTemplate("{?param1,param2}");
            var actual = template.GetResolver().Bind("param2", "test").ResolveTemplate().ToString();
            Assert.AreEqual("?param2=test{&param1}", actual);
        }

        [Test]
        public void PartialResolveAllVariablesTest([Values("#", "+", "")] string exprOp)
        {
            var template = new UriTemplate("{" + exprOp + "param1,param2}");
            var partialTemplate = template.GetResolver().Bind("param1", "param1").Bind("param2", "param2").ResolveTemplate();
            var expected = (exprOp == "+" ? string.Empty : exprOp) + "param1,param2";
            Assert.AreEqual(expected, partialTemplate.ToString());
        }

        [Test]
        public void PartialResolveNothingTest([Values("#", "+", "")] string exprOp)
        {
            var template = new UriTemplate("{" + exprOp + "param1,param2}");
            var partialTemplate = template.GetResolver().Bind("unknown", "1").ResolveTemplate();

            Assert.AreEqual(template.ToString(), partialTemplate.ToString());
        }

        [Test]
        public void ResolveIsNotAvailableForPartialVariablesTest([Values("#", "+", "")] string exprOp)
        {
            var template = new UriTemplate("{" + exprOp + "param1,param2}");
            Assert.Throws<UriTemplateException>(() => template.GetResolver().Bind("param1", "test").ResolveTemplate());
            Assert.Throws<UriTemplateException>(() => template.GetResolver().Bind("param2", "test").ResolveTemplate());
        }
    }
}