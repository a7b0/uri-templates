using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Resta.UriTemplates.Tests
{
    public abstract class SpecBaseTests
    {
        protected abstract string FileName { get; }

        protected virtual IEnumerable<TestCase> Samples => GetTestCases(true);

        protected virtual IEnumerable<TestCase> InvalidSamples => GetTestCases(false);

        
        [Fact]
        public void SpecTests()
        {
            foreach (var testCase in Samples)
            {
                SpecTest(testCase);
            }
        }

        // TODO: Write attribute that will generate different tests
        private static void SpecTest(TestCase testCase)
        {
            var uriTemplate = new UriTemplate(testCase.Template);
            var uri = uriTemplate.Resolve(testCase.Suite.Variables);
            Assert.Contains(uri, testCase.Expecteds);
        }

        [Fact]
        public void SpecInvalidTests()
        {
            foreach (var testCase in InvalidSamples)
            {
                SpecInvalidTest(testCase);
            }
        }

        // TODO: Write attribute that will generate different tests
        private static void SpecInvalidTest(TestCase testCase)
        {
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

            Assert.Empty("Template must be invalid");
        }

        private IEnumerable<TestCase> GetTestCases(bool valid)
        {
            return TestSuite.Load(FileName).SelectMany(x => x.TestCases).Where(x => x.IsInvalid == !valid);
        }
    }
}