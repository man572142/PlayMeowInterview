using System.Collections.Generic;
using System.Threading.Tasks;
using PlayMeow.Network;
using UnityEngine;

namespace PlayMeow.Auth
{
    /// <summary>
    /// Singleton service that handles login, auto-login, and logout.
    /// Uses <see cref="GraphQLClient"/> for network requests and
    /// <see cref="TokenStore"/> for local token persistence.
    /// </summary>
    public class AuthService
    {
        private const string Username = "username";
        private const string Password = "password";
        private static AuthService _instance;
        public static AuthService Instance => _instance ??= new AuthService();

        private readonly GraphQLClient _client;

        public string CurrentToken { get; private set; }
        public UserInfo CurrentUser { get; private set; }

        private AuthService()
        {
            _client = new GraphQLClient();
        }

        internal AuthService(GraphQLClient client)
        {
            _client = client;
        }

        // -----------------------------------------------------------------------
        // Login
        // -----------------------------------------------------------------------

        /// <summary>
        /// Authenticate with username + password via the GraphQL login mutation.
        /// Saves the token to PlayerPrefs on success.
        /// </summary>
        public async Task<LoginResult> LoginAsync(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return LoginResult.Fail("請填寫帳號和密碼");
            }

            const string query = @"
                mutation Login($username: String!, $password: String!) {
                    login(username: $username, password: $password) {
                        token
                        user { id username }
                    }
                }";

            var variables = new Dictionary<string, string>
            {
                { Username, username },
                { Password, password }
            };

            var response = await _client.QueryAsync(query, variables);
            return ProcessAuthResponse(response, AuthMode.Login);
        }

        // -----------------------------------------------------------------------
        // Signup
        // -----------------------------------------------------------------------

        /// <summary>
        /// Register a new account via the GraphQL signup mutation.
        /// Saves the token to PlayerPrefs on success (auto-login after signup).
        /// </summary>
        public async Task<LoginResult> SignupAsync(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return LoginResult.Fail("請填寫帳號和密碼");
            }

            const string query = @"
                mutation Signup($username: String!, $password: String!) {
                    signup(username: $username, password: $password) {
                        token
                        user { id username }
                    }
                }";

            var variables = new Dictionary<string, string>
            {
                { Username, username },
                { Password, password }
            };

            var response = await _client.QueryAsync(query, variables);
            return ProcessAuthResponse(response, AuthMode.SignUp);
        }

        // -----------------------------------------------------------------------
        // Auto-login (token validation)
        // -----------------------------------------------------------------------

        /// <summary>
        /// Validate the stored token by calling the <c>me</c> query.
        /// Returns a successful <see cref="LoginResult"/> when the token is still valid.
        /// Clears the stored token if validation fails.
        /// </summary>
        public async Task<LoginResult> AutoLoginAsync()
        {
            string token = TokenStore.Load();
            if (token == null)
            {
                return LoginResult.Fail("No stored token");
            }

            const string query = "{ me { id username } }";

            var response = await _client.QueryAsync(query, authToken: token);

            if (!string.IsNullOrEmpty(response.networkError))
            {
                Debug.LogWarning($"[AuthService] Auto-login network error: {response.networkError}");
                return LoginResult.Fail("網路連線失敗");
            }

            if (response.HasErrors || response.data?.me == null)
            {
                Debug.Log("[AuthService] Stored token is invalid; clearing.");
                TokenStore.Clear();
                return LoginResult.Fail("登入已過期，請重新登入");
            }

            CurrentToken = token;
            CurrentUser = new UserInfo
            {
                Id = response.data.me.id,
                Username = response.data.me.username
            };

            return LoginResult.Ok(token, CurrentUser);
        }

        // -----------------------------------------------------------------------
        // Google login (stub)
        // -----------------------------------------------------------------------

        /// <summary>
        /// Placeholder for Google OAuth login. Wire up the Google Sign-In SDK here.
        /// </summary>
        public async Task<LoginResult> GoogleLoginAsync()
        {
            await Task.Yield();
            Debug.Log("[AuthService] Google login not yet implemented.");
            return LoginResult.Fail("Google 登入尚未實作");
        }

        // -----------------------------------------------------------------------
        // Logout
        // -----------------------------------------------------------------------

        public void Logout()
        {
            CurrentToken = null;
            CurrentUser = null;
            TokenStore.Clear();
        }

        // -----------------------------------------------------------------------
        // Helpers
        // -----------------------------------------------------------------------

        internal LoginResult ProcessAuthResponse(GraphQLResponse response, AuthMode mode)
        {
            if (!string.IsNullOrEmpty(response.networkError))
            {
                Debug.LogWarning($"[AuthService] Network error: {response.networkError}");
                return LoginResult.Fail("網路連線失敗");
            }

            string defaultError = mode == AuthMode.SignUp ? "註冊失敗" : "帳號或密碼錯誤";

            // GraphQL returns HTTP 200 even on auth failure; always check errors[].
            if (response.HasErrors)
            {
                string msg = response.FirstError ?? defaultError;
                Debug.LogWarning($"[AuthService] GraphQL error: {msg}");
                return LoginResult.Fail(defaultError);
            }

            var payload = mode == AuthMode.SignUp
                ? response.data?.signup
                : response.data?.login;

            string token = payload?.token;
            if (string.IsNullOrEmpty(token))
            {
                Debug.LogWarning($"[AuthService] {mode} succeeded but token was empty.");
                return LoginResult.Fail(defaultError);
            }

            var graphqlUser = payload.user;
            var user = graphqlUser != null
                ? new UserInfo { Id = graphqlUser.id, Username = graphqlUser.username }
                : null;

            CurrentToken = token;
            CurrentUser = user;
            TokenStore.Save(token);

            return LoginResult.Ok(token, user);
        }
    }
}