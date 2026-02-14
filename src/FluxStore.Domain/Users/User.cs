using FluxStore.Domain.Abstractions;
using FluxStore.Domain.Core.Primitives;
using FluxStore.Domain.Core.Primitives.Result;

namespace FluxStore.Domain.Users
{
    public sealed class User : IUser
    {
        public string Id { get; private set; } // related to the application user.
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Username { get; private set; }
        public string Email { get; private set; }

        private User(string id, string firstName, string lastName, string username, string email)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Username = username;
            Email = email;
        }

        // for EF Core
        private User()
        {
        }

        public static Result<User> Create(string id, string firstName, string lastName, string username, string email)
        {

            var Result = new List<Error>();


            if (string.IsNullOrWhiteSpace(id))
            {
                Result.Add(Errors.UserErrors.IdIsRequired);
            }

            if (string.IsNullOrWhiteSpace(firstName))
            {
                Result.Add(Errors.UserErrors.FirstNameIsRequired);
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                Result.Add(Errors.UserErrors.LastNameIsRequired);
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                Result.Add(Errors.UserErrors.UsernameIsRequired);
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                Result.Add(Errors.UserErrors.EmailIsRequired);
            }

            if (Result.Any())
            {
                return Result<User>.Failure(Result);
            }

            return Result<User>.Success(new User(id, firstName, lastName, username, email));
        }

    }
}
