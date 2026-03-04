using Ardalis.Result;
using FluxStore.Api.Domain;
using FluxStore.Api.Shared.Errors;
using Microsoft.AspNetCore.Identity;

namespace FluxStore.Api.Shared.Extensions
{

    public static class IdentityResultExtensions
    {

        public static Result ToResult(this IdentityResult result)
        {
            if (result.Succeeded)
            {
                return Result.NoContent();
            }
            string identifier = string.Empty;
            string message = string.Empty;
            string code = string.Empty;

            var firstError = result.Errors.FirstOrDefault();
            var firstCode = firstError?.Code;

            switch (firstCode)
            {
                case "DuplicateEmail" or "DuplicateUserName":
                    return Result.Conflict(IdentityErrors.EmailAlreadyExists.Code, IdentityErrors.EmailAlreadyExists.Description);
                case "DuplicateRoleName":
                    return Result.Conflict(IdentityErrors.DuplicateRoleName.Code, IdentityErrors.DuplicateRoleName.Description);
                case "ConcurrencyFailure":
                    return Result.Conflict(IdentityErrors.ConcurrencyFailure.Code, IdentityErrors.ConcurrencyFailure.Description);
                case "DefaultError":
                    return Result.Error(IdentityErrors.DefaultError.Description);
                case "InvalidRoleName":
                    {
                        var err = IdentityErrors.InvalidRoleName;
                        var id = err.Code.Split("__").ElementAtOrDefault(1) ?? string.Empty;
                        return Result.Invalid(new List<ValidationError> { new ValidationError(id, err.Description, err.Code, default) });
                    }
                case "InvalidUserName":
                    {
                        var err = IdentityErrors.InvalidUserName;
                        var id = err.Code.Split("__").ElementAtOrDefault(1) ?? string.Empty;
                        return Result.Invalid(new List<ValidationError> { new ValidationError(id, err.Description, err.Code, default) });
                    }
                case "LoginAlreadyAssociated":
                    return Result.Conflict(IdentityErrors.LoginAlreadyAssociated.Code, IdentityErrors.LoginAlreadyAssociated.Description);
                case "InvalidToken":
                    return Result.Unauthorized(IdentityErrors.InvalidToken.Code, IdentityErrors.InvalidToken.Description);
                case "PasswordMismatch":
                    identifier = IdentityErrors.InvalidPassword.Code.Split("__").ElementAtOrDefault(1) ?? string.Empty;
                    message = IdentityErrors.InvalidPassword.Description;
                    code = IdentityErrors.InvalidPassword.Code;
                    return Result.Unauthorized(message, code);
                case "PasswordTooShort":
                    {
                        var err = IdentityErrors.PasswordTooShort;
                        var id = err.Code.Split("__").ElementAtOrDefault(1) ?? string.Empty;
                        return Result.Invalid(new List<ValidationError> { new ValidationError(id, err.Description, err.Code, default) });
                    }
                case "PasswordRequiresDigit":
                    {
                        var err = IdentityErrors.PasswordRequiresDigit;
                        var id = err.Code.Split("__").ElementAtOrDefault(1) ?? string.Empty;
                        return Result.Invalid(new List<ValidationError> { new ValidationError(id, err.Description, err.Code, default) });
                    }
                case "PasswordRequiresLower":
                    {
                        var err = IdentityErrors.PasswordRequiresLower;
                        var id = err.Code.Split("__").ElementAtOrDefault(1) ?? string.Empty;
                        return Result.Invalid(new List<ValidationError> { new ValidationError(id, err.Description, err.Code, default) });
                    }
                case "PasswordRequiresUpper":
                    {
                        var err = IdentityErrors.PasswordRequiresUpper;
                        var id = err.Code.Split("__").ElementAtOrDefault(1) ?? string.Empty;
                        return Result.Invalid(new List<ValidationError> { new ValidationError(id, err.Description, err.Code, default) });
                    }
                case "PasswordRequiresNonAlphanumeric":
                    {
                        var err = IdentityErrors.PasswordRequiresNonAlphanumeric;
                        var id = err.Code.Split("__").ElementAtOrDefault(1) ?? string.Empty;
                        return Result.Invalid(new List<ValidationError> { new ValidationError(id, err.Description, err.Code, default) });
                    }
                case "PasswordRequiresUniqueChars":
                    {
                        var err = IdentityErrors.PasswordRequiresUniqueChars;
                        var id = err.Code.Split("__").ElementAtOrDefault(1) ?? string.Empty;
                        return Result.Invalid(new List<ValidationError> { new ValidationError(id, err.Description, err.Code, default) });
                    }
                case "InvalidEmail":
                    identifier = UserErrors.InvalidEmail.Code.Split("__").ElementAtOrDefault(1) ?? string.Empty;
                    message = UserErrors.InvalidEmail.Description;
                    code = UserErrors.InvalidEmail.Code;
                    return Result.Invalid(new List<ValidationError> { new ValidationError(identifier, message, code, default) });
                case "RecoveryCodeRedemptionFailed":
                    identifier = IdentityErrors.RecoveryCodeRedemptionFailed.Code.Split("__").ElementAtOrDefault(1) ?? string.Empty;
                    message = IdentityErrors.RecoveryCodeRedemptionFailed.Description;
                    code = IdentityErrors.RecoveryCodeRedemptionFailed.Code;
                    return Result.Invalid(new List<ValidationError> { new ValidationError(identifier, message, code, default) });
                case "UserAlreadyHasPassword":
                    return Result.Conflict(IdentityErrors.UserAlreadyHasPassword.Code, IdentityErrors.UserAlreadyHasPassword.Description);
                case "UserLockoutNotEnabled":
                    return Result.Invalid(new List<ValidationError> { new ValidationError(IdentityErrors.UserLockoutNotEnabled.Code.Split("__").ElementAtOrDefault(1) ?? string.Empty, IdentityErrors.UserLockoutNotEnabled.Description, IdentityErrors.UserLockoutNotEnabled.Code, default) });
                case "UserIdNotFound":
                    return Result.NotFound(IdentityErrors.UserIdNotFound.Code, IdentityErrors.UserIdNotFound.Description);
                case "UserNameNotFound":
                    return Result.NotFound(IdentityErrors.UserNameNotFound.Code, IdentityErrors.UserNameNotFound.Description);
                default:
                    message = firstError?.Description ?? "Unknown";
                    return Result.Error(message);
            }
        }
        public static Result<T> ToResult<T>(this IdentityResult result) where T : class
        {
            if (result.Succeeded)
            {
                return Result.NoContent();
            }

            string identifier = string.Empty;
            string message = string.Empty;
            string code = string.Empty;


            var firstError = result.Errors.FirstOrDefault();
            var firstCode = firstError?.Code;

            switch (firstCode)
            {
                case "DuplicateEmail" or "DuplicateUserName":
                    return Result.Conflict(IdentityErrors.EmailAlreadyExists.Code, IdentityErrors.EmailAlreadyExists.Description);
                case "DuplicateRoleName":
                    return Result.Conflict(IdentityErrors.DuplicateRoleName.Code, IdentityErrors.DuplicateRoleName.Description);
                case "ConcurrencyFailure":
                    return Result.Conflict(IdentityErrors.ConcurrencyFailure.Code, IdentityErrors.ConcurrencyFailure.Description);
                case "DefaultError":
                    return Result.Error(IdentityErrors.DefaultError.Description);
                case "InvalidRoleName":
                    {
                        var err = IdentityErrors.InvalidRoleName;
                        var id = err.Code.Split("__").ElementAtOrDefault(1) ?? string.Empty;
                        return Result.Invalid(new List<ValidationError> { new ValidationError(id, err.Description, err.Code, default) });
                    }
                case "InvalidUserName":
                    {
                        var err = IdentityErrors.InvalidUserName;
                        var id = err.Code.Split("__").ElementAtOrDefault(1) ?? string.Empty;
                        return Result.Invalid(new List<ValidationError> { new ValidationError(id, err.Description, err.Code, default) });
                    }
                case "LoginAlreadyAssociated":
                    return Result.Conflict(IdentityErrors.LoginAlreadyAssociated.Code, IdentityErrors.LoginAlreadyAssociated.Description);
                case "InvalidToken":
                    return Result.Unauthorized(IdentityErrors.InvalidToken.Code, IdentityErrors.InvalidToken.Description);
                case "PasswordMismatch":
                    identifier = IdentityErrors.InvalidPassword.Code.Split("__").ElementAtOrDefault(1) ?? string.Empty;
                    message = IdentityErrors.InvalidPassword.Description;
                    code = IdentityErrors.InvalidPassword.Code;
                    return Result.Invalid(new List<ValidationError> { new ValidationError(identifier, message, code, default) });
                case "PasswordTooShort":
                    {
                        var err = IdentityErrors.PasswordTooShort;
                        var id = err.Code.Split("__").ElementAtOrDefault(1) ?? string.Empty;
                        return Result.Invalid(new List<ValidationError> { new ValidationError(id, err.Description, err.Code, default) });
                    }
                case "PasswordRequiresDigit":
                    {
                        var err = IdentityErrors.PasswordRequiresDigit;
                        var id = err.Code.Split("__").ElementAtOrDefault(1) ?? string.Empty;
                        return Result.Invalid(new List<ValidationError> { new ValidationError(id, err.Description, err.Code, default) });
                    }
                case "PasswordRequiresLower":
                    {
                        var err = IdentityErrors.PasswordRequiresLower;
                        var id = err.Code.Split("__").ElementAtOrDefault(1) ?? string.Empty;
                        return Result.Invalid(new List<ValidationError> { new ValidationError(id, err.Description, err.Code, default) });
                    }
                case "PasswordRequiresUpper":
                    {
                        var err = IdentityErrors.PasswordRequiresUpper;
                        var id = err.Code.Split("__").ElementAtOrDefault(1) ?? string.Empty;
                        return Result.Invalid(new List<ValidationError> { new ValidationError(id, err.Description, err.Code, default) });
                    }
                case "PasswordRequiresNonAlphanumeric":
                    {
                        var err = IdentityErrors.PasswordRequiresNonAlphanumeric;
                        var id = err.Code.Split("__").ElementAtOrDefault(1) ?? string.Empty;
                        return Result.Invalid(new List<ValidationError> { new ValidationError(id, err.Description, err.Code, default) });
                    }
                case "PasswordRequiresUniqueChars":
                    {
                        var err = IdentityErrors.PasswordRequiresUniqueChars;
                        var id = err.Code.Split("__").ElementAtOrDefault(1) ?? string.Empty;
                        return Result.Invalid(new List<ValidationError> { new ValidationError(id, err.Description, err.Code, default) });
                    }
                case "InvalidEmail":
                    identifier = UserErrors.InvalidEmail.Code.Split("__").ElementAtOrDefault(1) ?? string.Empty;
                    message = UserErrors.InvalidEmail.Description;
                    code = UserErrors.InvalidEmail.Code;
                    return Result.Invalid(new List<ValidationError> { new ValidationError(identifier, message, code, default) });
                case "RecoveryCodeRedemptionFailed":
                    identifier = IdentityErrors.RecoveryCodeRedemptionFailed.Code.Split("__").ElementAtOrDefault(1) ?? string.Empty;
                    message = IdentityErrors.RecoveryCodeRedemptionFailed.Description;
                    code = IdentityErrors.RecoveryCodeRedemptionFailed.Code;
                    return Result.Invalid(new List<ValidationError> { new ValidationError(identifier, message, code, default) });
                case "UserAlreadyHasPassword":
                    return Result.Conflict(IdentityErrors.UserAlreadyHasPassword.Code, IdentityErrors.UserAlreadyHasPassword.Description);
                case "UserLockoutNotEnabled":
                    return Result.Invalid(new List<ValidationError> { new ValidationError(IdentityErrors.UserLockoutNotEnabled.Code.Split("__").ElementAtOrDefault(1) ?? string.Empty, IdentityErrors.UserLockoutNotEnabled.Description, IdentityErrors.UserLockoutNotEnabled.Code, default) });
                case "UserIdNotFound":
                    return Result.NotFound(IdentityErrors.UserIdNotFound.Code, IdentityErrors.UserIdNotFound.Description);
                case "UserNameNotFound":
                    return Result.NotFound(IdentityErrors.UserNameNotFound.Code, IdentityErrors.UserNameNotFound.Description);
                default:
                    message = firstError?.Description ?? "Unknown";
                    return Result.Error(message);
            }
        }
        public static Error ToError(this IdentityError error)
        {
            return error.Code switch
            {

                // ===== Duplicate / Exists =====
                "DuplicateEmail" =>
                    IdentityErrors.EmailAlreadyExists,

                "DuplicateUserName" =>
                    IdentityErrors.UsernameAlreadyExists,

                "DuplicateRoleName" =>
                    IdentityErrors.DuplicateRoleName,

                "ConcurrencyFailure" =>
                    IdentityErrors.ConcurrencyFailure,

                // ===== Invalid input =====
                "InvalidEmail" =>
                    UserErrors.InvalidEmail,

                "InvalidUserName" =>
                    UserErrors.InvalidUsername,
                "InvalidRoleName" =>
                    IdentityErrors.InvalidRoleName,
                "PasswordMismatch" =>
                IdentityErrors.InvalidPassword,

                // ===== Password =====
                "PasswordTooShort" =>
                    IdentityErrors.PasswordTooShort,

                "PasswordRequiresDigit" =>
                    IdentityErrors.PasswordRequiresDigit,

                "PasswordRequiresLower" =>
                    IdentityErrors.PasswordRequiresLower,

                "PasswordRequiresUpper" =>
                    IdentityErrors.PasswordRequiresUpper,

                "PasswordRequiresNonAlphanumeric" =>
                    IdentityErrors.PasswordRequiresNonAlphanumeric,

                "PasswordReuseNotAllowed" =>
                    IdentityErrors.PasswordReuseNotAllowed,

                "PasswordRequiresUniqueChars" =>
                    IdentityErrors.PasswordRequiresUniqueChars,

                // ===== Security / Tokens =====
                "InvalidToken" =>
                    IdentityErrors.InvalidToken,

                "LoginAlreadyAssociated" =>
                    IdentityErrors.LoginAlreadyAssociated,

                "UserAlreadyInRole" =>
                    IdentityErrors.UserAlreadyInRole,

                "UserNotInRole" =>
                    IdentityErrors.UserNotInRole,

                "UserAlreadyHasPassword" =>
                    IdentityErrors.UserAlreadyHasPassword,

                "UserLockoutNotEnabled" =>
                    IdentityErrors.UserLockoutNotEnabled,

                "UserIdNotFound" =>
                    IdentityErrors.UserIdNotFound,

                "UserNameNotFound" =>
                    IdentityErrors.UserNameNotFound,

                "RecoveryCodeRedemptionFailed" =>
                    IdentityErrors.RecoveryCodeRedemptionFailed,


                // ===== Fallback =====
                _ =>
                    IdentityErrors.Unknown
            };
        }



    }
}
