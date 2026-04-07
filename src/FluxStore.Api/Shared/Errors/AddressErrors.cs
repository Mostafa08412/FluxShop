using FluxStore.Api.Domain;

namespace FluxStore.Api.Shared.Errors
{
    public static class AddressErrors
    {
        public static Error NotFound =>
            new("Address_NotFound__AddressId", "Address not found.", ErrorType.NotFound);


        public static Error IdRequired =>
            new("Address_AddressIdRequired__AddressId", "Address id is required.", ErrorType.Validation);

        public static Error UserIdRequired =>
            new("Address_UserIdRequired__UserId", "User id is required.", ErrorType.Validation);

        public static Error LabelRequired =>
            new("Address_LabelRequired__Label", "Label is required.", ErrorType.Validation);

        public static Error LabelTooLong =>
            new("Address_LabelTooLong__Label", "Label must not exceed 50 characters.", ErrorType.Validation);

        public static Error StreetRequired =>
            new("Address_StreetRequired__Street", "Street is required.", ErrorType.Validation);

        public static Error StreetTooLong =>
            new("Address_StreetTooLong__Street", "Street must not exceed 200 characters.", ErrorType.Validation);

        public static Error CityRequired =>
            new("Address_CityRequired__City", "City is required.", ErrorType.Validation);

        public static Error CityTooLong =>
            new("Address_CityTooLong__City", "City must not exceed 100 characters.", ErrorType.Validation);

        public static Error StateTooLong =>
            new("Address_StateTooLong__State", "State must not exceed 100 characters.", ErrorType.Validation);

        public static Error CountryRequired =>
            new("Address_CountryRequired__Country", "Country is required.", ErrorType.Validation);

        public static Error CountryTooLong =>
            new("Address_CountryTooLong__Country", "Country must not exceed 100 characters.", ErrorType.Validation);

        public static Error PostalCodeRequired =>
            new("Address_PostalCodeRequired__PostalCode", "Postal code is required.", ErrorType.Validation);

        public static Error PostalCodeTooLong =>
            new("Address_PostalCodeTooLong__PostalCode", "Postal code must not exceed 20 characters.", ErrorType.Validation);

        public static Error LimitExceeded =>
            new("Address_LimitExceeded__Addresses", "Maximum 10 addresses allowed.", ErrorType.Failure);

        public static Error CannotDeleteOnlyDefault =>
            new("Address_CannotDeleteOnlyDefault__AddressId", "Cannot delete the only default address.", ErrorType.ConditionNotMet);

        public static Error CannotSetDeletedAsDefault =>
            new("Address_CannotSetDeletedAsDefault__AddressId", "Cannot set deleted address as default.", ErrorType.ConditionNotMet);


        public static string[] LimitExceededError =>
       [LimitExceeded.Code, LimitExceeded.Description];
        public static string[] NotFoundError =>
       [NotFound.Code, NotFound.Description];

        public static string[] CannotDeleteOnlyDefaultError =>
            [CannotDeleteOnlyDefault.Code, CannotDeleteOnlyDefault.Description];

        public static string[] CannotSetDeletedAsDefaultError =>
            [CannotSetDeletedAsDefault.Code, CannotSetDeletedAsDefault.Description];
    }
}
