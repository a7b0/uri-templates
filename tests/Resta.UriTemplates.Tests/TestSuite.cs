using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Resta.UriTemplates.Tests
{
    public class TestSuite
    {
        public string Name { get; set; }

        public string Level { get; set; }

        public Dictionary<string, object> Variables { get; set; }

        public List<TestCase> TestCases { get; set; }

        public static List<TestSuite> Load(string fileName)
        {
            var testSuites = new List<TestSuite>();

            using (var stream = File.OpenRead(fileName))
            {
                var root = JObject.Load(new JsonTextReader(new StreamReader(stream)));
                testSuites.AddRange(root.Children<JProperty>().Select(item => CreateTestSuite(item.Name, item.Value)));
            }

            return testSuites;
        }

        private static TestSuite CreateTestSuite(string name, JToken token)
        {
            var testSuite = new TestSuite
            {
                Name = name,
                TestCases = new List<TestCase>(),
                Variables = new Dictionary<string, object>()
            };

            foreach (var variable in token.SelectToken("variables").Cast<JProperty>())
            {
                var value = CreateValue(variable.Value);
                testSuite.Variables.Add(variable.Name, value);
            }

            foreach (var testCase in token.SelectToken("testcases").Children<JArray>())
            {
                var value = CreateTestCase(testCase);
                value.Suite = testSuite;
                testSuite.TestCases.Add(value);
            }

            return testSuite;
        }

        private static TestCase CreateTestCase(JArray items)
        {
            var template = items[0].Value<string>();
            var isInvalid = false;
            var expecteds = new List<string>();

            if (items[1].Type == JTokenType.Array)
            {
                expecteds.AddRange(items[1].Select(x => x.Value<string>()));
            }
            else if (items[1].Type == JTokenType.String)
            {
                expecteds.Add(items[1].Value<string>());
            }
            else if (items[1].Type == JTokenType.Boolean)
            {
                isInvalid = !items[1].Value<bool>();
            }
            else
            {
                throw new FormatException("Invalid testcases format");
            }

            return new TestCase
            {
                Template = template,
                IsInvalid = isInvalid,
                Expecteds = expecteds
            };
        }

        private static object CreateValue(JToken token)
        {
            if (token.Type == JTokenType.Array)
            {
                return token.Select(x => x.Value<string>()).ToList();
            }
            if (token.Type == JTokenType.Object)
            {
                return token
                    .Children<JProperty>()
                    .ToDictionary(x => x.Name, x => x.Value.Value<string>());
            }
            if (token is JValue)
            {
                var value = token.Value<string>();

                if (token.Type == JTokenType.Float)
                {
                    value = value.Replace(',', '.');
                }

                return value;
            }
            if (token.Type == JTokenType.Null)
            {
                return null;
            }

            throw new FormatException(string.Format("Unexpected type of varspec \"{0}\"", token.Type));
        }
    }
}