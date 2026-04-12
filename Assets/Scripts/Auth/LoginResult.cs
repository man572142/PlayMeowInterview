namespace PlayMeow.Auth
{
    public class LoginResult
    {
        public bool Success { get; private set; }
        public string ErrorMessage { get; private set; }
        public string Token { get; private set; }
        public UserInfo User { get; private set; }

        public static LoginResult Ok(string token, UserInfo user = null) =>
            new LoginResult { Success = true, Token = token, User = user };

        public static LoginResult Fail(string errorMessage) =>
            new LoginResult { Success = false, ErrorMessage = errorMessage };
    }
}