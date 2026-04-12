using System.Collections;
using System.Threading.Tasks;
using NUnit.Framework;
using PlayMeow.Auth;
using PlayMeow.Network;
using UnityEngine;
using UnityEngine.TestTools;

namespace PlayMeow.Tests
{
    public class AuthServiceTests
    {
        private AuthService _service;

        [SetUp]
        public void SetUp()
        {
            _service = new AuthService(new GraphQLClient());
        }

        [TearDown]
        public void TearDown()
        {
            PlayerPrefs.DeleteKey(TokenStore.TokenKey);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Yield-wait for a Task to complete, pumping Unity's main loop each frame.
        /// </summary>
        private static IEnumerator WaitForTask<T>(Task<T> task)
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }

            if (task.IsFaulted)
            {
                throw task.Exception.InnerException ?? task.Exception;
            }
        }

        // Validation tests (no network calls)

        [UnityTest]
        public IEnumerator LoginAsync_EmptyUsername_ReturnsFail()
        {
            var task = _service.LoginAsync("", "password");
            yield return WaitForTask(task);
            var result = task.Result;

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.ErrorMessage);
        }

        [UnityTest]
        public IEnumerator LoginAsync_EmptyPassword_ReturnsFail()
        {
            var task = _service.LoginAsync("username", "");
            yield return WaitForTask(task);
            var result = task.Result;

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.ErrorMessage);
        }

        [UnityTest]
        public IEnumerator SignupAsync_EmptyUsername_ReturnsFail()
        {
            var task = _service.SignupAsync("", "password");
            yield return WaitForTask(task);
            var result = task.Result;

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.ErrorMessage);
        }

        [UnityTest]
        public IEnumerator SignupAsync_EmptyPassword_ReturnsFail()
        {
            var task = _service.SignupAsync("user@example.com", "");
            yield return WaitForTask(task);
            var result = task.Result;

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.ErrorMessage);
        }

        [UnityTest]
        public IEnumerator GoogleLoginAsync_ReturnsNotImplementedFail()
        {
            var task = _service.GoogleLoginAsync();
            yield return WaitForTask(task);
            var result = task.Result;

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.ErrorMessage);
        }

        [Test]
        public void Logout_ClearsTokenAndUser()
        {
            // Arrange: simulate a logged-in state via ProcessAuthResponse
            var response = new GraphQLResponse
            {
                data = new GraphQLData
                {
                    login = new AuthPayload
                    {
                        token = "tok",
                        user = new GraphQLUser { id = "1", username = "cat" }
                    }
                }
            };
            _service.ProcessAuthResponse(response, AuthMode.Login);

            _service.Logout();

            Assert.IsNull(_service.CurrentToken);
            Assert.IsNull(_service.CurrentUser);
        }

        // ProcessAuthResponse unit tests

        [Test]
        public void ProcessAuthResponse_NetworkError_ReturnsFail()
        {
            var response = new GraphQLResponse { networkError = "timeout" };

            var result = _service.ProcessAuthResponse(response, AuthMode.Login);

            Assert.IsFalse(result.Success);
        }

        [Test]
        public void ProcessAuthResponse_GraphQLErrors_ReturnsFail()
        {
            var response = new GraphQLResponse
            {
                errors = new[] { new GraphQLError { message = "invalid credentials" } }
            };

            var result = _service.ProcessAuthResponse(response, AuthMode.Login);

            Assert.IsFalse(result.Success);
        }

        [Test]
        public void ProcessAuthResponse_ValidResponse_ReturnsOkWithToken()
        {
            var response = new GraphQLResponse
            {
                data = new GraphQLData
                {
                    login = new AuthPayload
                    {
                        token = "valid-token",
                        user = new GraphQLUser { id = "5", username = "meow" }
                    }
                }
            };

            var result = _service.ProcessAuthResponse(response, AuthMode.Login);

            Assert.IsTrue(result.Success);
            Assert.AreEqual("valid-token", result.Token);
            Assert.AreEqual("5", result.User.Id);
            Assert.AreEqual("meow", result.User.Username);
        }

        [Test]
        public void ProcessAuthResponse_EmptyToken_ReturnsFail()
        {
            var response = new GraphQLResponse
            {
                data = new GraphQLData
                {
                    login = new AuthPayload { token = "" }
                }
            };

            var result = _service.ProcessAuthResponse(response, AuthMode.Login);

            Assert.IsFalse(result.Success);
        }
    }
}