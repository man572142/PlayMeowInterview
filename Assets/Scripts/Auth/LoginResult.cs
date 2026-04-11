namespace PlayMeow.Auth
{
    public class LoginResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string Token { get; set; }
        public UserInfo User { get; set; }

        public static LoginResult Ok(string token, UserInfo user = null) =>
            new LoginResult { Success = true, Token = token, User = user };

        public static LoginResult Fail(string errorMessage) =>
            new LoginResult { Success = false, ErrorMessage = errorMessage };
    }
}
