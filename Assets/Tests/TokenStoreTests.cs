using NUnit.Framework;
using PlayMeow.Auth;
using UnityEngine;

namespace PlayMeow.Tests
{
    public class TokenStoreTests
    {
        private const string MyToken = "my-token";
        private const string AnyToken = "any-token";

        [SetUp]
        public void SetUp()
        {
            PlayerPrefs.DeleteKey(TokenStore.TokenKey);
            PlayerPrefs.Save();
        }

        [TearDown]
        public void TearDown()
        {
            PlayerPrefs.DeleteKey(TokenStore.TokenKey);
            PlayerPrefs.Save();
        }

        [Test]
        public void SaveAndLoad_RoundTrips()
        {
            TokenStore.Save(MyToken);

            Assert.AreEqual(MyToken, TokenStore.Load());
        }

        [Test]
        public void Load_NoToken_ReturnsNull()
        {
            Assert.IsNull(TokenStore.Load());
        }

        [Test]
        public void HasToken_AfterSave_ReturnsTrue()
        {
            TokenStore.Save(AnyToken);

            Assert.IsTrue(TokenStore.HasToken());
        }

        [Test]
        public void Clear_RemovesToken()
        {
            TokenStore.Save(MyToken);
            TokenStore.Clear();

            Assert.IsNull(TokenStore.Load());
            Assert.IsFalse(TokenStore.HasToken());
        }
    }
}