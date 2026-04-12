using NUnit.Framework;
using PlayMeow.Network;

namespace PlayMeow.Tests
{
    public class GraphQLResponseTests
    {
        [Test]
        public void HasErrors_NoErrors_ReturnsFalse()
        {
            var response = new GraphQLResponse { errors = new GraphQLError[0] };

            Assert.IsFalse(response.HasErrors);
        }

        [Test]
        public void HasErrors_NullErrors_ReturnsFalse()
        {
            var response = new GraphQLResponse { errors = null };

            Assert.IsFalse(response.HasErrors);
        }

        [Test]
        public void HasErrors_WithErrors_ReturnsTrue()
        {
            var response = new GraphQLResponse
            {
                errors = new[] { new GraphQLError { message = "oops" } }
            };

            Assert.IsTrue(response.HasErrors);
        }

        [Test]
        public void FirstError_ReturnsMessage()
        {
            var response = new GraphQLResponse
            {
                errors = new[] { new GraphQLError { message = "first error" } }
            };

            Assert.AreEqual("first error", response.FirstError);
        }

        [Test]
        public void FirstError_NoErrors_ReturnsNull()
        {
            var response = new GraphQLResponse { errors = null };

            Assert.IsNull(response.FirstError);
        }
    }
}