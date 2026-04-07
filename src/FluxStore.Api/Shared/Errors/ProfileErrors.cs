using FluxStore.Api.Domain;

namespace FluxStore.Api.Shared.Errors
{
    public static class ProfileErrors
    {
        public static Error DisplayNameRequired =>
            new("Profile_DisplayNameRequired__DisplayName", "Display name is required.", ErrorType.Validation);

        public static Error InvalidPhone =>
            new("Profile_InvalidPhone__Phone", "Phone must be in E.164 format.", ErrorType.Validation);

        public static Error DisplayNameTooLong =>
            new("Profile_DisplayNameTooLong__DisplayName", "Display name must not exceed 100 characters.", ErrorType.Validation);

        public static Error ProfileNotFound =>
            new("Profile_ProfileNotFound__UserId", "Profile not found.", ErrorType.NotFound);

        public static Error NotificationSettingsRequired =>
            new("Profile_NotificationSettingsRequired__NotificationSettings", "Notification settings are required.", ErrorType.Validation);


        public static string[] DisplayNameRequiredError => [DisplayNameRequired.Code, DisplayNameRequired.Description];
        public static string[] InvalidPhoneError => [InvalidPhone.Code, InvalidPhone.Description];
        public static string[] DisplayNameTooLongError => [DisplayNameTooLong.Code, DisplayNameTooLong.Description];
        public static string[] ProfileNotFoundError => [ProfileNotFound.Code, ProfileNotFound.Description];
        public static string[] NotificationSettingsRequiredError => [NotificationSettingsRequired.Code, NotificationSettingsRequired.Description];



    }
}
