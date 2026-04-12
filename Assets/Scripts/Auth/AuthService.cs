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
                return LoginResult.Fail("login_error_fill_required");
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
            return ProcessAuthResponse(response, "login");
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
                return LoginResult.Fail("login_error_fill_required");
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
                { "username", username },
                { "password", password }
            };

            GraphQLResponse response = await _client.QueryAsync(query, variables);
            return ProcessAuthResponse(response, "signup");
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

            GraphQLResponse response = await _client.QueryAsync(query, authToken: token);

            if (!string.IsNullOrEmpty(response.networkError))
            {
                Debug.LogWarning($"[AuthService] Auto-login network error: {response.networkError}");
                return LoginResult.Fail("login_error_network");
            }

            if (response.HasErrors || response.data?.me == null)
            {
                Debug.Log("[AuthService] Stored token is invalid; clearing.");
                TokenStore.Clear();
                return LoginResult.Fail("login_error_session_expired");
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
            return LoginResult.Fail("login_error_google_not_implemented");
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

        internal LoginResult ProcessAuthResponse(GraphQLResponse response, string operation)
        {
            if (!string.IsNullOrEmpty(response.networkError))
            {
                Debug.LogWarning($"[AuthService] Network error: {response.networkError}");
                return LoginResult.Fail("login_error_network");
            }

            string defaultError = operation == "signup" ? "login_error_signup_failed" : "login_error_wrong_credentials";

            // GraphQL returns HTTP 200 even on auth failure; always check errors[].
            if (response.HasErrors)
            {
                string msg = response.FirstError ?? defaultError;
                Debug.LogWarning($"[AuthService] GraphQL error: {msg}");
                return LoginResult.Fail(defaultError);
            }

            AuthPayload payload = operation == "signup"
                ? response.data?.signup
                : response.data?.login;

            string token = payload?.token;
            if (string.IsNullOrEmpty(token))
            {
                Debug.LogWarning($"[AuthService] {operation} succeeded but token was empty.");
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