using System.Collections.Generic;
using NUnit.Framework;
using PlayMeow.Network;

namespace PlayMeow.Tests
{
    public class GraphQLClientTests
    {
        // JsonString tests

        [Test]
        public void JsonString_NullInput_ReturnsNull()
        {
            Assert.AreEqual("null", GraphQLClient.JsonString(null));
        }

        [Test]
        public void JsonString_PlainString_WrapsInQuotes()
        {
            Assert.AreEqual("\"hello\"", GraphQLClient.JsonString("hello"));
        }

        [Test]
        public void JsonString_EscapesBackslash()
        {
            Assert.AreEqual("\"a\\\\b\"", GraphQLClient.JsonString("a\\b"));
        }

        [Test]
        public void JsonString_EscapesQuotes()
        {
            Assert.AreEqual("\"say \\\"hi\\\"\"", GraphQLClient.JsonString("say \"hi\""));
        }

        [Test]
        public void JsonString_EscapesNewlineAndTab()
        {
            Assert.AreEqual("\"\\n\\t\"", GraphQLClient.JsonString("\n\t"));
        }

        // BuildRequestJson tests

        [Test]
        public void BuildRequestJson_QueryOnly_ProducesValidJson()
        {
            string json = GraphQLClient.BuildRequestJson("{ me }", null);

            Assert.AreEqual("{\"query\":\"{ me }\"}", json);
        }

        [Test]
        public void BuildRequestJson_WithVariables_IncludesVariablesObject()
        {
            var vars = new Dictionary<string, string> { { "username", "cat" } };
            string json = GraphQLClient.BuildRequestJson("mutation", vars);

            StringAssert.Contains("\"variables\":", json);
            StringAssert.Contains("\"username\"", json);
            StringAssert.Contains("\"cat\"", json);
        }
    }
}