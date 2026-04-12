using NUnit.Framework;
using PlayMeow.Auth;

namespace PlayMeow.Tests
{
    public class LoginResultTests
    {
        [Test]
        public void Ok_ReturnsSuccessWithToken()
        {
            var result = LoginResult.Ok("tok123");

            Assert.IsTrue(result.Success);
            Assert.AreEqual("tok123", result.Token);
            Assert.IsNull(result.ErrorMessage);
        }

        [Test]
        public void Ok_WithUser_SetsUser()
        {
            var user = new UserInfo { Id = "1", Username = "cat" };
            var result = LoginResult.Ok("tok123", user);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(user, result.User);
        }

        [Test]
        public void Fail_ReturnsFailureWithMessage()
        {
            var result = LoginResult.Fail("bad credentials");

            Assert.IsFalse(result.Success);
            Assert.AreEqual("bad credentials", result.ErrorMessage);
            Assert.IsNull(result.Token);
        }
    }
}