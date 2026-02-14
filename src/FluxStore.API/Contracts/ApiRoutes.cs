namespace FluxStore.Api.Contracts
{
    public static class ApiRoutes
    {

        public const string Root = "api/v{version:apiVersion}";
        public static class Authentication
        {
            public const string Base = Root + $"/auth";

            public const string Register = $"register";
            public const string Login = $"login";
            public const string Logout = $"logout";
            public const string RefreshToken = $"refresh-token";
            public const string ChangePassword = $"change-password";
            public const string ForgetPassword = $"forget-password";
            public const string VerifyResetPasswordOtp = $"verify-reset-password-otp";
            public const string ResetPassword = $"reset-password";
        }

        public static class Users
        {
            public const string Base = Root + $"/users";
            public const string GetById = "{userId}";
            public const string Create = "";
            public const string Update = "{userId}";
            public const string Activate = "{userId}/activate";
            public const string Deactivate = "{userId}/deactivate";
        }

    }
}
