using NUnit.Framework;
using PlayMeow.Auth;

namespace PlayMeow.Tests
{
    public class UserInfoTests
    {
        [Test]
        public void Properties_CanBeSetAndRead()
        {
            var user = new UserInfo { Id = "42", Username = "meow" };

            Assert.AreEqual("42", user.Id);
            Assert.AreEqual("meow", user.Username);
        }
    }
}