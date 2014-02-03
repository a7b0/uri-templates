using System.Collections.Generic;
using NUnit.Framework;
using Resta.UriTemplates;

namespace UriTemplates.Tests
{
    [TestFixture]
    public class PartialResolveTests
    {
        [Test]
        public void PartialResolvePathOnlyWithAllParamsTest()
        {
            var template = new UriTemplate("http://example.com/{path1}/{path2}");

            var partiallyResolved = template.ResolveUriTemplate(new Dictionary<string, object>() {{"path1", "test"}, {"path2", "test2"}});
            
            Assert.AreEqual("http://example.com/test/test2", partiallyResolved.Template);
        }

        [Test]
        public void PartialResolvePathOnlyWithSomeParamsTest()
        {
            var template = new UriTemplate("http://example.com/{path1}/{path2}");

            var partiallyResolved = template.ResolveUriTemplate(new Dictionary<string, object>() { { "path1", "test" } });

            Assert.AreEqual("http://example.com/test/{path2}", partiallyResolved.Template);
        }

        [Test]
        public void PartialResolveContinuationWithSomeParamsTest()
        {
            var template = new UriTemplate("http://example.com/{path1}/{path2}?test=abc{&query1,query2}");

            var partiallyResolved = template.ResolveUriTemplate(new Dictionary<string, object>() { { "path1", "test" }, {"query1", "testq1"} });

            Assert.AreEqual("http://example.com/test/{path2}?test=abc&query1=testq1{&query2}", partiallyResolved.Template);
        }

        [Test]
        public void PartialResolveQueryWithSomeParamsTest()
        {
            var template = new UriTemplate("http://example.com/{path1}/{path2}{?query1,query2}");

            var partiallyResolved = template.ResolveUriTemplate(new Dictionary<string, object>() { { "path1", "test" }, { "query1", "testq1" } });

            Assert.AreEqual("http://example.com/test/{path2}?query1=testq1{&query2}", partiallyResolved.Template);
        }

        [Test]
        public void PartialResolveQueryWithNullParamsTest()
        {
            var template = new UriTemplate("http://example.com/{path1}/{path2}{?query1,query2}");

            var partiallyResolved = template.ResolveUriTemplate(new Dictionary<string, object>() { { "path1", "test" }, { "query1", "testq1" }, { "query2", null} });

            Assert.AreEqual("http://example.com/test/{path2}?query1=testq1", partiallyResolved.Template);
        }
    }
}