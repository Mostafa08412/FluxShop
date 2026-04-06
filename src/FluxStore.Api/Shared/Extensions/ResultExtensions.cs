using Ardalis.Result;
using FluxStore.Api.Resources;
using FluxStore.Api.Shared.Models;
using Microsoft.Extensions.Localization;

namespace FluxStore.Api.Shared.Extensions
{
    public static class ResultExtensions
    {


        public static ApiResponse<T> ToApiResponse<T>(this PagedResult<T> result, HttpContext context, IStringLocalizer<ErrorsResource>? stringLocalizer = null, int? statusCodeOnSuccess = null)
        {
            if (result.IsSuccess)
            {
                Dictionary<string, string> meta = new Dictionary<string, string>
                {
                    { "TotalCount", result.PagedInfo.TotalRecords.ToString() },
                    { "PageSize", result.PagedInfo.PageSize.ToString() },
                    { "Page", result.PagedInfo.PageNumber.ToString() },
                    { "TotalPages", result.PagedInfo.TotalPages.ToString() }
                };

                return ApiResponse<T>.Success(
                    data: result.Value,
                    context: context,
                    message: result.SuccessMessage,
                    statusCode: statusCodeOnSuccess ?? MapArdalisResultStatusToStatusCode(result.Status),
                    meta: meta);
            }

            return HandleFailure(result, context);
        }

        public static ApiResponse<T> ToApiResponse<T>(this Result<T> result, HttpContext context, IStringLocalizer<ErrorsResource>? stringLocalizer = null, int? statusCodeOnSuccess = null)
        {
            if (result.IsSuccess)
            {
                return ApiResponse<T>.Success(
                    context: context,
                    data: result.Value,
                    message: result.SuccessMessage,
                    statusCode: statusCodeOnSuccess ?? MapArdalisResultStatusToStatusCode(result.Status)
                );
            }

            // Even for generic results, we return the base ApiResponse on failure
            return HandleFailure<T>(result, context, stringLocalizer);
        }

        #region Private Helper Functions
        private static IDictionary<string, string> ToValidationErrorsDict(this Ardalis.Result.IResult result, IStringLocalizer<ErrorsResource>? stringLocalizer = null)
        {
            if (result.ValidationErrors == null || !result.ValidationErrors.Any())
            {
                return new Dictionary<string, string>();
            }

            return result.ValidationErrors.DistinctBy(X => X.Identifier).ToDictionary(
                error => error.Identifier ??
                            error.ErrorCode?.Split("__").ElementAtOrDefault(1) ?? "General",
                error => stringLocalizer?.GetString(error.ErrorCode) ?? error.ErrorMessage
            );
        }
        // --- Private Helper for Generic Failures ---
        private static ApiResponse<T> HandleFailure<T>(Result<T> result, HttpContext context, IStringLocalizer<ErrorsResource>? stringLocalizer = null)
        {
            var validationErrors = result.ToValidationErrorsDict(stringLocalizer);
            var statusCode = MapArdalisResultStatusToStatusCode(result.Status);
            var errorCode = result.Errors.ElementAtOrDefault(0) ?? "ERROR";
            var message = stringLocalizer?.GetString(errorCode) ?? result.Errors.ElementAtOrDefault(1) ?? "An error occurred";

            if (result.IsInvalid())
            {
                message = "One or more validation errors occurred.";
                errorCode = "VALIDATION_ERROR";
            }

            // Return the generic record with default data (null)
            return new ApiResponse<T>(
                data: default!,
                isSuccess: false,
                message: message,
                errorCode: errorCode,
                statusCode: statusCode,
                validationErrors: validationErrors,
                meta: new Dictionary<string, string>(),
                context: context
            );
        }


        private static int MapArdalisResultStatusToStatusCode(ResultStatus status) => status switch
        {
            ResultStatus.Ok => 200,
            ResultStatus.Created => 201,
            ResultStatus.NoContent => 204,
            ResultStatus.Invalid => 400,
            ResultStatus.Unauthorized => 401,
            ResultStatus.Forbidden => 403,
            ResultStatus.NotFound => 404,
            ResultStatus.Conflict => 409,
            ResultStatus.Error => 422,
            ResultStatus.CriticalError => 500,
            ResultStatus.Unavailable => 503,
            _ => 500
        };
        #endregion
    }

}
