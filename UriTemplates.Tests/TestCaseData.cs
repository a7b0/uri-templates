using System.Collections.Generic;
using System.Linq;

namespace Resta.UriTemplates.Tests
{
    public static class TestCaseData
    {
        public static IEnumerable<TestCase> GetSamples(string fileName)
        {
            return GetTestCases(fileName, true);
        }

        public static IEnumerable<TestCase> GetInvalidSamples(string fileName)
        {
            return GetTestCases(fileName, false);
        }

        private static IEnumerable<TestCase> GetTestCases(string fileName, bool valid)
        {
            return TestSuite.Load(fileName).SelectMany(x => x.TestCases).Where(x => x.IsInvalid == !valid);
        }
    }
}