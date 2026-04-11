using UnityEngine;

namespace PlayMeow.Auth
{
    /// <summary>
    /// Persists the JWT auth token to PlayerPrefs so it survives app restarts.
    /// </summary>
    public static class TokenStore
    {
        private const string TokenKey = "pm_auth_token";

        /// <summary>Save the token to PlayerPrefs.</summary>
        public static void Save(string token)
        {
            PlayerPrefs.SetString(TokenKey, token);
            PlayerPrefs.Save();
        }

        /// <summary>Load the saved token, or null if none exists.</summary>
        public static string Load()
        {
            string t = PlayerPrefs.GetString(TokenKey, string.Empty);
            return string.IsNullOrEmpty(t) ? null : t;
        }

        /// <summary>True when a token has been persisted.</summary>
        public static bool HasToken() => Load() != null;

        /// <summary>Remove the stored token (on logout or invalid token).</summary>
        public static void Clear()
        {
            PlayerPrefs.DeleteKey(TokenKey);
            PlayerPrefs.Save();
        }
    }
}
