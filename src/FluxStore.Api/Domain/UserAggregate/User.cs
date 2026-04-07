using Ardalis.Result;
using FluxStore.Api.Domain.UserAggregate.Events;
using FluxStore.Api.Domain.UserAggregate.ValueObjects;
using FluxStore.Api.Shared.Errors;
using FluxStore.Api.Shared.Extensions;

namespace FluxStore.Api.Domain.UserAggregate
{
    public sealed class User : Aggregate
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Username { get; private set; }
        public string Email { get; private set; }
        public string DisplayName { get; private set; }
        public PhoneNumber? Phone { get; private set; }
        public string? AvatarUrl { get; private set; }
        public NotificationSettings NotificationSettings { get; private set; }

        private User(
            Guid id,
            string firstName,
            string lastName,
            string username,
            string email,
            string? displayName = null,
            PhoneNumber? phone = null,
            string? avatarUrl = null,
            NotificationSettings? notificationSettings = null) : base(id)
        {
            FirstName = firstName;
            LastName = lastName;
            Username = username;
            Email = email;
            DisplayName = displayName ?? $"{firstName} {lastName}".Trim();
            Phone = phone;
            AvatarUrl = avatarUrl;
            NotificationSettings = notificationSettings ?? NotificationSettings.Default;
        }

        // for EF Core
        private User()
        {
        }

        public static Result<User> Create(Guid id, string firstName, string lastName, string username, string email)
        {
            var errors = new List<Error>();

            if (id == Guid.Empty)
            {
                errors.Add(UserErrors.IdIsRequired);
            }

            if (string.IsNullOrWhiteSpace(firstName))
            {
                errors.Add(UserErrors.NameIsRequired);
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                errors.Add(UserErrors.LastNameIsRequired);
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                errors.Add(UserErrors.UsernameIsRequired);
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                errors.Add(UserErrors.EmailAddressIsRequired);
            }

            if (errors.Any())
            {
                return Result<User>.Invalid(errors.Select(X => new ValidationError(X.Code, X.Description)));
            }

            return Result<User>.Success(new User(id, firstName, lastName, username, email));
        }

        public Result UpdateProfile(string firstName, string lastName)
        {
            var errors = new List<Error>();

            if (string.IsNullOrWhiteSpace(firstName))
            {
                errors.Add(UserErrors.NameIsRequired);
            }
            else if (firstName.Length > 100)
            {
                errors.Add(UserErrors.FirstNameTooLong);
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                errors.Add(UserErrors.LastNameIsRequired);
            }
            else if (lastName.Length > 100)
            {
                errors.Add(UserErrors.LastNameTooLong);
            }

            if (errors.Any())
            {
                return Result.Invalid(errors.Select(X => X.ToValidationError()));
            }

            FirstName = firstName;
            LastName = lastName;
            DisplayName = $"{firstName} {lastName}".Trim();

            return Result.Success();
        }

        public Result UpdateProfile(string displayName, PhoneNumber? phone, string? avatarUrl)
        {
            var errors = new List<Error>();

            if (string.IsNullOrWhiteSpace(displayName))
            {
                errors.Add(ProfileErrors.DisplayNameRequired);
            }
            else if (displayName.Length > 100)
            {
                errors.Add(ProfileErrors.DisplayNameTooLong);
            }

            if (errors.Any())
            {
                return Result.Invalid(errors.Select(X => X.ToValidationError()));
            }

            DisplayName = displayName;
            Phone = phone;
            AvatarUrl = avatarUrl;

            RaiseDomainEvent(new ProfileUpdated(Id, DisplayName, Phone?.Value, AvatarUrl));

            return Result.Success();
        }

        public Result UpdateNotificationSettings(NotificationSettings settings)
        {
            if (settings is null)
            {
                return Result.Invalid(ProfileErrors.NotificationSettingsRequired.ToValidationError());
            }

            NotificationSettings = settings;
            RaiseDomainEvent(new NotificationSettingsChanged(Id, settings));
            return Result.Success();
        }

        public Result Logout(string? deviceId)
        {
            RaiseDomainEvent(new UserLoggedOut(Id, deviceId));
            return Result.Success();
        }

    }

}
