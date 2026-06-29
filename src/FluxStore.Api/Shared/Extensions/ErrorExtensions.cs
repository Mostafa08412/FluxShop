using Ardalis.Result;
using FluentValidation.Results;
using FluxStore.Api.Domain;

namespace FluxStore.Api.Shared.Extensions
{
    public static class ErrorExtensions
    {

        public static string[] ToErrorList(this Error error)
        {
            return [error.Code, error.Description];
        }
        public static ValidationError ToValidationError(this Error error)
        {

            return new ValidationError
            {
                ErrorCode = error.Code,
                ErrorMessage = error.Description,
                Identifier = error.Code.Split("__").Skip(1).FirstOrDefault() ?? "General"

            };

        }

        public static ValidationError ToValidationError(this ValidationFailure error)
        {

            return new ValidationError
            {
                ErrorCode = error.ErrorCode,
                ErrorMessage = error.ErrorMessage,
                Identifier = error.PropertyName ?? error.ErrorCode.Split("__").ElementAtOrDefault(1) ?? "General"

            };

        }
    }
}
