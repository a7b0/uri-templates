namespace Resta.UriTemplates.Tests
{
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public abstract class SpecBaseTests
    {
        public abstract string FileName { get; }

        public virtual IEnumerable<TestCase> Samples
        {
            get { return GetTestCases(true); }
        }

        public virtual IEnumerable<TestCase> InvalidSamples
        {
            get { return GetTestCases(false); }
        }

        [Test, TestCaseSource("Samples")]
        public void SpecTest(TestCase testCase)
        {
            Assume.That(!testCase.IsInvalid);

            var uriTemplate = new UriTemplate(testCase.Template);
            var uri = uriTemplate.Resolve(testCase.Suite.Variables);
            Assert.Contains(uri, testCase.Expecteds);
        }

        [Test, TestCaseSource("InvalidSamples")]
        public void SpecInvalidTest(TestCase testCase)
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

        private IEnumerable<TestCase> GetTestCases(bool valid)
        {
            return TestSuite.Load(FileName).SelectMany(x => x.TestCases).Where(x => x.IsInvalid == !valid);
        }
    }
}