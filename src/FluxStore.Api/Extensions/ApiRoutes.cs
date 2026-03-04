namespace FluxStore.Api.Extensions
{
    public static class ApiRoutes
    {

        public const string BaseUrl = "/api/";

        #region Authentication Endpoints
        public const string Version = "v1";
        public const string Authentication = BaseUrl + $"{Version}/auth";
        public const string Login = "login";
        public const string Logout = "logout";
        public const string Register = "register";
        public const string LoginWithGoogle = "google-login";
        public const string ResetPassword = "reset-password";
        public const string ChangePassword = "change-password";
        public const string VerifyOtp = "verify-reset-password-otp";
        public const string ForgetPassword = "forget-password";
        public const string RefreshToken = "refresh-token";
        #endregion


    }
}
