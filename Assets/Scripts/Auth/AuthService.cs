using System.Threading.Tasks;
using PlayMeow.Network;
using UnityEngine;

namespace PlayMeow.Auth
{
    public class AuthService
    {
        private static AuthService _instance;
        public static AuthService Instance => _instance ??= new AuthService();

        private readonly GraphQLClient _client;

        private AuthService()
        {
            _client = new GraphQLClient();
        }

        public async Task<LoginResult> LoginAsync(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return LoginResult.Fail("請填寫帳號和密碼");

            const string query = @"
                mutation Login($email: String!, $password: String!) {
                    login(email: $email, password: $password) {
                        token
                    }
                }";

            var variables = new { email, password };

            var response = await _client.QueryAsync(query, variables);

            if (!string.IsNullOrEmpty(response.error))
            {
                Debug.LogWarning($"[AuthService] Login error: {response.error}");
                return LoginResult.Fail("帳號或密碼錯誤");
            }

            if (response.data?.login?.token == null)
                return LoginResult.Fail("帳號或密碼錯誤");

            return LoginResult.Ok(response.data.login.token);
        }

        /// <summary>
        /// Placeholder for Google OAuth login. Implement with Google Sign-In SDK.
        /// </summary>
        public async Task<LoginResult> GoogleLoginAsync()
        {
            await Task.Yield();
            Debug.Log("[AuthService] Google login not yet implemented.");
            return LoginResult.Fail("Google 登入尚未實作");
        }
    }
}
