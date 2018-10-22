using NUnit.Framework;

namespace Resta.UriTemplates.Tests
{
    [TestFixture]
    
    public class SpecExampleTest
    {
        private const string FileName = "testcases/spec-examples.json";

        [Test, TestCaseSource(typeof(TestCaseData), nameof(TestCaseData.GetSamples), new object[] { FileName })]
        protected void SpecTest(TestCase testCase)
        {
            SpecBaseTests.SpecTest(testCase);
        }

        [Test, TestCaseSource(typeof(TestCaseData), nameof(TestCaseData.GetInvalidSamples), new object[] { FileName })]
        protected void SpecInvalidTest(TestCase testCase)
        {
            SpecBaseTests.SpecInvalidTest(testCase);
        }
    }
}