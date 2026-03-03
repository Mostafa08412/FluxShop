using FluxStore.Domain.Core.Primitives;

namespace FluxStore.Domain.Core.Errors
{
    public static partial class Errors
    {


        public static class UserErrors
        {
            public static Error FirstNameIsRequired =>
        new("Identity_FirstNameIsRequired__Name", "First name is required.", ErrorType.Validation);

            public static Error LastNameIsRequired =>
                new("Identity_LastNameIsRequired__Name", "Last name is required.", ErrorType.Validation);

            public static Error FirstNameTooLong =>
                new("Identity_FirstNameTooLong__Name", "First name must not exceed 100 characters.", ErrorType.Validation);

            public static Error LastNameTooLong =>
                new("Identity_LastNameTooLong__Name", "Last name must not exceed 100 characters.", ErrorType.Validation);

            public static Error NameIsRequired =>
                new("Identity_NameIsRequired__Name", "Name is required.", ErrorType.Validation);

            public static Error InvalidNameFormat =>
                new("Identity_InvalidNameFormat__Name", "Name must consist of first name and last name separated by a space.", ErrorType.Validation);

            public static Error InvalidEmail =>
    new("Identity_InvalidEmail__EmailAddress", "The email format is invalid. Please enter a valid email address.", ErrorType.Validation);

            public static Error InvalidUsername =>
                new("Identity_InvalidUsername__Username", "The username contains invalid characters or does not meet the required length.", ErrorType.Validation);
            public static Error EmailIsRequired =>
                      new("Identity_EmailIsRequired__EmailAddress", "Email address is required.", ErrorType.Validation);


            public static Error IdIsRequired => new Error("User_IdIsRequired__UserId", "User Id is required", ErrorType.Validation);
            public static Error UsernameIsRequired => new Error("User_UsernameIsRequired__Username", "Username is required", ErrorType.Validation);
            public static Error EmailAddressIsRequired => new Error("User_EmailAddressIsRequired__EmailAddress", "Email is required", ErrorType.Validation);
            public static Error InvalidEmailAddress => new Error("User_InvalidEmailAddress__EmailAddress", "Invalid email format", ErrorType.Validation);
            public static Error EmailAddressTooLong => new Error("User_EmailAddressTooLong__EmailAddress", "Email must not exceed 256 characters", ErrorType.Validation);
            public static Error InvalidRole => new Error("User_InvalidRole__Role", "Role must be Admin, Manager, or User", ErrorType.Validation);
            public static Error RoleIsRequired => new Error("User_RoleIsRequired__Role", "Role is required", ErrorType.Validation);
        }

    }
}
