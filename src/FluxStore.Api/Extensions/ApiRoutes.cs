namespace FluxStore.Api.Extensions
{
    public static class ApiRoutes
    {
        public static class Authentication
        {
            #region Authentication Endpoints

            public const string Prefix = "auth";

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
        public static class Account
        {
            #region Account Management Endpoints

            public const string Prefix = "account";

            public const string Profile = "profile";
            public const string Addresses = "addresses";
            public const string PaymentMethods = "payment-methods";
            public const string Settings = "settings";


            public const string AddressById = Addresses + "/{id}";
            public const string AddressDefault = Addresses + "/default";

            public const string PaymentMethodById = PaymentMethods + "/{id}";
            public const string PaymentMethodDefault = PaymentMethodById + "/default";

            public const string NotificationSettings = Settings + "/notifications";
            #endregion

        }


    }
}
