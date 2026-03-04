using FluxStore.Api.Domain;

namespace FluxStore.Api.Shared.Errors
{
    public static class IdentityErrors
    {
        // ----- User Search & Identification -----

        public static Error UserNotFoundByEmail =>
            new("Identity_UserNotFound__Email", $"No account found associated with the email address.", ErrorType.NotFound);

        // ----- Authentication & Credentials -----
        public static Error InvalidCredentials =>
            new("Identity_InvalidCredentials", "Authentication failed. Please verify your username and password.", ErrorType.IdentityError);

        public static Error InvalidPassword =>
            new("Identity_InvalidPassword", "The password provided is incorrect. Please try again.", ErrorType.IdentityError);

        public static Error InvalidGoogleIdToken =>
            new("Identity_InvalidGoogleToken", "The Google authentication token is invalid or has expired.", ErrorType.IdentityError);

        // ----- Validation: Required Fields -----

        public static Error PasswordIsRequired =>
            new("Identity_PasswordIsRequired__Password", "Password is required.", ErrorType.Validation);

        public static Error CurrentPasswordIsRequired =>
            new("Identity_CurrentPasswordIsRequired__CurrentPassword", "Current password is required.", ErrorType.Validation);

        public static Error NewPasswordIsRequired =>
            new("Identity_NewPasswordIsRequired__NewPassword", "New password is required.", ErrorType.Validation);

        public static Error ConfirmNewPasswordIsRequired =>
            new("Identity_ConfirmNewPasswordIsRequired__ConfirmNewPassword", "Confirm new password is required.", ErrorType.Validation);

        public static Error ConfirmPasswordMismatch =>
            new("Identity_ConfirmPasswordMismatch__ConfirmNewPassword", "New password and confirm password do not match.", ErrorType.Validation);


        public static Error GoogleIdTokenIsRequired =>
            new("Identity_GoogleIdTokenIsRequired__IdToken", "Google ID token is required.", ErrorType.Validation);

        public static Error RefreshTokenIsRequired =>
            new("Identity_RefreshTokenIsRequired__RefreshToken", "Refresh token is required.", ErrorType.Validation);

        public static Error OtpIsRequired =>
            new("Identity_OtpIsRequired__Otp", "OTP code is required.", ErrorType.Validation);

        public static Error ResetPasswordTokenIsRequired =>
            new("Identity_ResetPasswordTokenIsRequired__ResetPasswordToken", "Reset password token is required.", ErrorType.Validation);

        // ----- Account Conflicts -----


        public static Error EmailAlreadyExists =>
            new("Identity_EmailAlreadyExists", "This email address is already registered. Please use a different email or sign in.", ErrorType.Conflict);

        public static Error UsernameAlreadyExists =>
            new("Identity_UsernameAlreadyExists", "This username is unavailable. Please choose another.", ErrorType.Conflict);

        // ----- Validation & Security Policy -----

        public static Error WeakPassword =>
            new("Identity_WeakPassword__Password", "Password does not meet the minimum security requirements (complexity/length).", ErrorType.Validation);

        // Specific password requirement errors
        public static Error PasswordTooShort =>
            new("Identity_PasswordTooShort__Password", "The password is too short.", ErrorType.Validation);

        public static Error PasswordRequiresDigit =>
            new("Identity_PasswordRequiresDigit__Password", "The password must contain at least one numeric digit.", ErrorType.Validation);

        public static Error PasswordRequiresLower =>
            new("Identity_PasswordRequiresLower__Password", "The password must contain at least one lowercase letter.", ErrorType.Validation);

        public static Error PasswordRequiresUpper =>
            new("Identity_PasswordRequiresUpper__Password", "The password must contain at least one uppercase letter.", ErrorType.Validation);

        public static Error PasswordRequiresNonAlphanumeric =>
            new("Identity_PasswordRequiresNonAlphanumeric__Password", "The password must contain at least one non-alphanumeric character.", ErrorType.Validation);

        public static Error PasswordReuseNotAllowed =>
            new("Identity_PasswordReuseNotAllowed__Password", "For security reasons, you cannot reuse a previously used password.", ErrorType.Validation);

        // ----- Status & Permissions -----
        public static Error ForbiddenAccess =>
            new("Identity_Forbidden", "Access Denied. You do not have the required permissions for this resource.", ErrorType.AccessDenied);

        public static Error UserLockout =>
            new("Identity_UserLockedOut", "This account has been temporarily locked due to multiple failed login attempts.", ErrorType.AccessDenied);

        public static Error RoleNotFound =>
            new("Identity_RoleNotFound", "The specified security role does not exist.", ErrorType.NotFound);

        public static Error UserAlreadyInRole =>
            new("Identity_UserAlreadyInRole", "The user is already assigned to the specified role.", ErrorType.Conflict);

        public static Error UserNotInRole =>
            new("Identity_UserNotInRole", "The user does not possess the specified role assignment.", ErrorType.NotFound);

        // ----- Token Management -----
        public static Error MissingToken =>
            new("Identity_MissingToken", "Authentication token is missing. Please provide a valid Authorization header.", ErrorType.IdentityError);

        public static Error InvalidToken =>
            new("Identity_InvalidToken", "The provided authentication token is malformed or invalid.", ErrorType.IdentityError);

        public static Error ExpiredToken =>
            new("Identity_ExpiredToken", "Your session has expired. Please log in again to continue.", ErrorType.IdentityError);

        public static Error RecoveryCodeRedemptionFailed =>
            new("Identity_RecoveryCodeRedemptionFailed", "The recovery code provided is invalid or has already been used.", ErrorType.Validation);

        // ===== Common IdentityResult codes added =====
        public static Error ConcurrencyFailure =>
            new("Identity_ConcurrencyFailure", "An optimistic concurrency failure occurred. The resource was modified by another process.", ErrorType.Conflict);

        public static Error DefaultError =>
            new("Identity_DefaultError", "An unspecified identity error occurred.", ErrorType.Failure);

        public static Error DuplicateRoleName =>
            new("Identity_DuplicateRoleName", "A role with the provided name already exists.", ErrorType.Conflict);

        public static Error InvalidRoleName =>
            new("Identity_InvalidRoleName__Role", "The provided role name is invalid.", ErrorType.Validation);

        public static Error InvalidUserName =>
            new("Identity_InvalidUserName__UserName", "The provided username is invalid.", ErrorType.Validation);

        public static Error LoginAlreadyAssociated =>
            new("Identity_LoginAlreadyAssociated", "A login for the specified external provider is already associated with an account.", ErrorType.Conflict);

        public static Error PasswordRequiresUniqueChars =>
            new("Identity_PasswordRequiresUniqueChars__Password", "The password must contain a minimum number of unique characters.", ErrorType.Validation);

        public static Error UserAlreadyHasPassword =>
            new("Identity_UserAlreadyHasPassword", "The user already has a password set.", ErrorType.Conflict);

        public static Error UserLockoutNotEnabled =>
            new("Identity_UserLockoutNotEnabled", "Lockout is not enabled for this user.", ErrorType.Validation);

        public static Error UserNotFound =>
            new("Identity_UserNotFound", "No user could be located with the provided identifier.", ErrorType.NotFound);

        public static Error UserIdNotFound =>
            new("Identity_UserIdNotFound", "No user could be located with the provided identifier.", ErrorType.NotFound);

        public static Error UserNameNotFound =>
            new("Identity_UserNameNotFound", "No user could be located with the provided username.", ErrorType.NotFound);

        // ----- Password Reset -----
        public static Error OtpCooldown =>
            new($"Identity_OtpCooldown__Otp", "Please wait 30 seconds before requesting another OTP.", ErrorType.Validation);

        public static Error InvalidOtp =>
            new($"Identity_InvalidOtp__Otp", "The OTP code entered is invalid or has expired.", ErrorType.Validation);

        public static Error InvalidResetToken =>
            new($"Identity_InvalidResetToken__ResetPasswordToken", "The password reset token is invalid or has expired.", ErrorType.Validation);

        // ----- Action Failures -----
        public static Error UpdateFailed =>
            new("Identity_UpdateFailed", "An error occurred while updating the user profile.", ErrorType.Failure);

        public static Error RoleUpdateFailed =>
            new("Identity_RoleUpdateFailed", "Failed to modify user role assignments.", ErrorType.Failure);

        public static Error DeactivationFailed =>
            new("Identity_DeactivationFailed", "Account deactivation process failed.", ErrorType.Failure);

        public static Error ActivationFailed =>
            new("Identity_ActivationFailed", "Account activation process failed.", ErrorType.Failure);

        public static Error Unknown =>
            new("Identity_Unknown", "An unexpected identity error occurred.", ErrorType.Failure);
    }
}
