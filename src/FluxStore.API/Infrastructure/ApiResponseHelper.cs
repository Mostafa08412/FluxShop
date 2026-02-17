using FluxStore.Api.Contracts;
using FluxStore.Application.Common.Models;
using FluxStore.Domain.Core.Primitives;
using FluxStore.Domain.Core.Primitives.Result;
using System.Collections.Immutable;


namespace FluxStore.Api.Infrastructure
{
    public class ApiResponseHelper
    {
        private readonly ImmutableDictionary<string, string> EmptyValidationErrors = ImmutableDictionary<string, string>.Empty;
        private readonly ImmutableDictionary<string, string> EmptyMetadata = ImmutableDictionary<string, string>.Empty;
        private Dictionary<string, string> MapValidationErrors(IEnumerable<Error> errors)
        {
            var validationErrors = new Dictionary<string, string>();
            foreach (var error in errors.Where(e => e.ErrorType == ErrorType.Validation))
            {
                validationErrors.TryAdd(error.Code, error.Description);
            }
            return validationErrors;
        }

        private (string errorCode, string message) GetErrorCodeAndMessage(IEnumerable<Error> errors)
        {
            var nonValidationError = errors.FirstOrDefault(x => x.ErrorType != ErrorType.Validation);

            if (nonValidationError is not null)
            {
                return (nonValidationError.Code, nonValidationError.Description);
            }
            if (errors.Any(e => e.ErrorType == ErrorType.Validation))
            {
                return ("VALIDATION_ERROR", "One or more validation errors have occurred.");
            }
            return (string.Empty, string.Empty);
        }


        public ApplicationStatusCodes CalculateStatusCodeFromResult(Result result, ApplicationStatusCodes onSuccess)
        {
            if (result.IsSuccess) return onSuccess;

            return result.Error!.ErrorType switch
            {
                ErrorType.Validation or ErrorType.Failure => ApplicationStatusCodes.BadRequest,
                ErrorType.IdentityError => ApplicationStatusCodes.Unauthorized,
                ErrorType.NotFound => ApplicationStatusCodes.NotFound,
                ErrorType.Conflict => ApplicationStatusCodes.Conflict,
                ErrorType.AccessDenied => ApplicationStatusCodes.Forbidden,
                ErrorType.ConditionNotMet => ApplicationStatusCodes.UnprocessableEntity,
                _ => ApplicationStatusCodes.InternalServerError
            };
        }

        public ApiResponse ResultToResponse(Result result, HttpContext context)
        {

            bool isSuccess = result.IsSuccess;

            (string errorCode, string message) = GetErrorCodeAndMessage(result.Errors);

            var validationErrors = MapValidationErrors(result.Errors);

            var response = new ApiResponse(
                isSuccess,
                message,
                errorCode,
                validationErrors,
                EmptyMetadata,
                context.Request.Path.Value ?? "Unknown",
                context.TraceIdentifier);

            return response;
        }

        public ApiResponse<T> ResultToResponse<T>(Result<T> result, HttpContext context)
        {

            bool isSuccess = result.IsSuccess;

            (string errorCode, string message) = GetErrorCodeAndMessage(result.Errors);

            Dictionary<string, string> validationErrors = MapValidationErrors(result.Errors);

            object? data = result.Value;

            if (data == null && isSuccess)
            {
                throw new InvalidOperationException("Result value cannot be null when creating a successful ApiResponse with data.");
            }

            if (result.Value is IPaginatedList<T> paginated)
            {
                Dictionary<string, string> meta = new();
                meta.Add("pageNumber", paginated.PageNumber.ToString());
                meta.Add("pageSize", paginated.PageSize.ToString());
                meta.Add("totalCount", paginated.TotalCount.ToString());
                meta.Add("totalPages", paginated.TotalPages.ToString());
                meta.Add("hasNext", paginated.HasNext.ToString().ToLowerInvariant());
                meta.Add("hasPrevious", paginated.HasPrevious.ToString().ToLowerInvariant());

                data = paginated.Items;

                if (data == null && isSuccess)
                    throw new InvalidOperationException("Result value cannot be null when creating a successful ApiResponse with data.");


                return new ApiResponse<T>(
                     isSuccess,
                     message,
                     errorCode,
                     validationErrors,
                     meta,
                     context.Request.Path.Value ?? "Unknown",
                     context.TraceIdentifier,
                     data: (T)data)
                {
                };

            }

            return new ApiResponse<T>(
                isSuccess,
                message,
                errorCode,
                validationErrors,
                EmptyMetadata,
                context.Request.Path.Value ?? "Unknown",
                context.TraceIdentifier,
                data: (T)data!)
            {
            };


        }


        public ApiResponse BasicErrorApiResponse(string message, string errorCode, string requestPath, string tracedIdentifier)
        {
            return new ApiResponse(false, message, errorCode, EmptyValidationErrors, EmptyMetadata, requestPath, tracedIdentifier);
        }

    }
}
