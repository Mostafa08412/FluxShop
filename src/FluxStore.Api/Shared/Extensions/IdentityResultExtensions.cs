using Ardalis.Result;
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

            var firstError = result.Errors.FirstOrDefault();
            var firstCode = firstError?.Code;

            switch (firstCode)
            {
                case "DuplicateEmail" or "DuplicateUserName":
                    return Result.Conflict(IdentityErrors.EmailAlreadyExists.ToErrorList());

                case "DuplicateRoleName":
                    return Result.Conflict(IdentityErrors.DuplicateRoleName.ToErrorList());

                case "InvalidToken":
                    return Result.Unauthorized(IdentityErrors.InvalidToken.ToErrorList());

                case "LoginAlreadyAssociated":
                    return Result.Conflict(IdentityErrors.LoginAlreadyAssociated.ToErrorList());

                case "ConcurrencyFailure":
                    return Result.Conflict(IdentityErrors.ConcurrencyFailure.ToErrorList());

                case "DefaultError":
                    return Result.Error(IdentityErrors.DefaultError.Description);

                case "InvalidRoleName":
                    return Result.Invalid([IdentityErrors.InvalidRoleName.ToValidationError()]);

                case "InvalidUserName":
                    return Result.Invalid([IdentityErrors.InvalidUserName.ToValidationError()]);

                case "PasswordMismatch":
                    return Result.Unauthorized(IdentityErrors.InvalidPassword.ToErrorList());

                case "PasswordTooShort":
                    return Result.Invalid([IdentityErrors.PasswordTooShort.ToValidationError()]);

                case "PasswordRequiresDigit":
                    return Result.Invalid([IdentityErrors.PasswordRequiresDigit.ToValidationError()]);

                case "PasswordRequiresLower":
                    return Result.Invalid([IdentityErrors.PasswordRequiresLower.ToValidationError()]);

                case "PasswordRequiresUpper":
                    return Result.Invalid([IdentityErrors.PasswordRequiresUpper.ToValidationError()]);

                case "PasswordRequiresNonAlphanumeric":
                    return Result.Invalid([IdentityErrors.PasswordRequiresNonAlphanumeric.ToValidationError()]);

                case "PasswordRequiresUniqueChars":
                    return Result.Invalid([IdentityErrors.PasswordRequiresUniqueChars.ToValidationError()]);

                case "InvalidEmail":
                    return Result.Invalid([UserErrors.InvalidEmail.ToValidationError()]);

                case "RecoveryCodeRedemptionFailed":
                    return Result.Invalid([IdentityErrors.RecoveryCodeRedemptionFailed.ToValidationError()]);

                case "UserAlreadyHasPassword":
                    return Result.Conflict(IdentityErrors.UserAlreadyHasPassword.ToErrorList());

                case "UserLockoutNotEnabled":
                    return Result.Invalid([IdentityErrors.UserLockoutNotEnabled.ToValidationError()]);

                case "UserIdNotFound":
                    return Result.NotFound(IdentityErrors.UserIdNotFound.ToErrorList());

                case "UserNameNotFound":
                    return Result.NotFound(IdentityErrors.UserNameNotFound.ToErrorList());

                default:
                    var message = firstError?.Description ?? "Unknown";
                    return Result.Error(message);
            }
        }


    }
}
