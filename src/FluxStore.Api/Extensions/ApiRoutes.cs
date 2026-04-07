namespace FluxStore.Api.Extensions
{
    public static class ApiRoutes
    {

        public const string BaseUrl = "/api/";
        public const string Versioned = BaseUrl + "v1";

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

        #region Account Management Endpoints
        public const string Profile = BaseUrl + $"{Version}/profile";
        public const string Addresses = "addresses";
        public const string AddressById = Addresses + "/{id}";
        public const string AddressDefault = Addresses + "/default";
        public const string PaymentMethods = "payment-methods";
        public const string PaymentMethodById = PaymentMethods + "/{id}";
        public const string PaymentMethodDefault = PaymentMethodById + "/default";
        public const string Settings = "settings";
        public const string NotificationSettings = Settings + "/notifications";
        #endregion


    }
}
