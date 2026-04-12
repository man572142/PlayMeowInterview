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
        private static AuthService _instance;
        public static AuthService Instance => _instance ??= new AuthService();

        private readonly GraphQLClient _client;

        // Publicly readable after a successful login/auto-login.
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
                { "username", username },
                { "password", password }
            };

            GraphQLResponse response = await _client.QueryAsync(query, variables);
            return ProcessAuthResponse(response);
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
            if (string.IsNullOrEmpty(token))
            {
                return LoginResult.Fail("No stored token");
            }

            const string query = "{ me { id username } }";

            GraphQLResponse response = await _client.QueryAsync(query, authToken: token);

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

            // Token is still valid — restore session state.
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

        internal LoginResult ProcessAuthResponse(GraphQLResponse response)
        {
            if (!string.IsNullOrEmpty(response.networkError))
            {
                Debug.LogWarning($"[AuthService] Network error: {response.networkError}");
                return LoginResult.Fail("網路連線失敗");
            }

            // GraphQL returns HTTP 200 even on auth failure; always check errors[].
            if (response.HasErrors)
            {
                string msg = response.FirstError ?? "帳號或密碼錯誤";
                Debug.LogWarning($"[AuthService] GraphQL error: {msg}");
                return LoginResult.Fail("帳號或密碼錯誤");
            }

            string token = response.data?.login?.token;
            if (string.IsNullOrEmpty(token))
            {
                Debug.LogWarning("[AuthService] Login succeeded but token was empty.");
                return LoginResult.Fail("帳號或密碼錯誤");
            }

            var graphqlUser = response.data.login.user;
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