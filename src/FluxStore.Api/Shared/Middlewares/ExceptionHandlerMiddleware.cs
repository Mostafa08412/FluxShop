using FluxStore.Api.Domain;
using FluxStore.Api.Resources;
using FluxStore.Api.Shared.Errors;
using FluxStore.Api.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace FluxStore.Api.Shared.Middlewares
{

    public static class ExceptionHandlerMiddlewareDI
    {

        public static void UseGloabalExceptionHandler(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
    public class ExceptionHandlerMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;
        private readonly IStringLocalizer<ErrorsResource> _localizer;


        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger, IStringLocalizer<ErrorsResource> localizer)
        {
            _next = next;
            _logger = logger;
            _localizer = localizer;
        }


        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception of type {ExceptionType} occurred: {Message}", ex.GetType().Name, ex.Message);

                (Error error, int statusCode) = MapExceptionToError(ex);

                await WriteResponseAsync(error, statusCode, context);
            }

        }


        private (Error, int) MapExceptionToError(Exception ex)
        {
            return ex switch
            {
                DbUpdateConcurrencyException => (IdentityErrors.ConcurrencyFailure, 409),
                DbUpdateException => (SystemErrors.DatabaseUpdateError, 409),
                ArgumentNullException => (SystemErrors.MissingConfiguration, 500),
                _ => (SystemErrors.UnexpectedError, 500)
            };
        }

        public async Task WriteResponseAsync(Error error, int statusCode, HttpContext context)
        {
            var localizedMessage = _localizer.GetString(error.Code);

            var message = string.IsNullOrEmpty(localizedMessage) ? error.Description : localizedMessage;

            var response = ApiResponse.Failure(context, message, error.Code, statusCode);

            context.Response.StatusCode = statusCode;

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
