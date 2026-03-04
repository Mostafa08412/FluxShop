using FluxStore.Api.Domain;

namespace FluxStore.Api.Shared.Errors
{
    public static class SystemErrors
    {
        public static Error DatabaseUpdateError => new("System_DatabaseUpdateError", "A database integrity error occurred.", ErrorType.Failure);
        public static Error MissingConfiguration => new("System_MissingConfiguration", "A required system configuration is missing.", ErrorType.Failure);
        public static Error UnexpectedError => new("System_UnexpectedError", "An internal server error occurred.", ErrorType.Failure);
    }
}
