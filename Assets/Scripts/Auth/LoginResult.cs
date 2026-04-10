namespace PlayMeow.Auth
{
    public class LoginResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string Token { get; set; }

        public static LoginResult Ok(string token) =>
            new LoginResult { Success = true, Token = token };

        public static LoginResult Fail(string errorMessage) =>
            new LoginResult { Success = false, ErrorMessage = errorMessage };
    }
}
