namespace Resta.UriTemplates.Tests
{
    using NUnit.Framework;
    using System;

    public static class SpecBaseTests
    {
        public static void SpecTest(TestCase testCase)
        {
            Assume.That(!testCase.IsInvalid);
            var uriTemplate = new UriTemplate(testCase.Template);
            var uri = uriTemplate.Resolve(testCase.Suite.Variables);
            Assert.Contains(uri, testCase.Expecteds);
        }

        public static void SpecInvalidTest(TestCase testCase)
        {
            Assume.That(testCase.IsInvalid);

            try
            {
                var uriTemplate = new UriTemplate(testCase.Template);
                var uri = uriTemplate.Resolve(testCase.Suite.Variables);
            }
            catch (Exception exception)
            {
                if (exception is UriTemplateException)
                {
                    return;
                }
            }

            Assert.Fail("Template must be invalid");
        }


    }
}