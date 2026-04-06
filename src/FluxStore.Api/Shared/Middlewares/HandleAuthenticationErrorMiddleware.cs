using FluxStore.Api.Resources;
using FluxStore.Api.Shared.Errors;
using FluxStore.Api.Shared.Models;
using Microsoft.Extensions.Localization;
using Error = FluxStore.Api.Domain.Error;
namespace FluxStore.Api.Shared.Middlewares
{

    public static class AuthErrorHandlerMiddlewareDI
    {

        public static void UseAuthErrorHandlingMiddleware(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<HandleAuthenticationErrorMiddleware>();
        }
    }

    public class HandleAuthenticationErrorMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IStringLocalizer<ErrorsResource> _localizer;

        public HandleAuthenticationErrorMiddleware(RequestDelegate next, IStringLocalizer<ErrorsResource> localizer)
        {
            this.next = next;
            _localizer = localizer;
        }

        public async Task InvokeAsync(HttpContext context)
        {


            await next.Invoke(context);

            if (context.Response.Headers.ContainsKey("Auth-Fail-Type"))
            {
                string? headerValue = context.Response.Headers["Auth-Fail-Type"];

                if (!string.IsNullOrEmpty(headerValue))
                {
                    Error error = IdentityErrors.InvalidToken;

                    if (headerValue == IdentityErrors.InvalidToken.Code)
                    {
                        error = IdentityErrors.InvalidToken;
                        context.Response.StatusCode = 401;
                    }
                    if (headerValue == IdentityErrors.MissingToken.Code)
                    {
                        error = IdentityErrors.MissingToken;
                        context.Response.StatusCode = 401;
                    }

                    if (headerValue == IdentityErrors.ExpiredToken.Code)
                    {
                        error = IdentityErrors.ExpiredToken;
                        context.Response.StatusCode = 401;
                    }

                    if (headerValue == IdentityErrors.ForbiddenAccess.Code)
                    {
                        error = IdentityErrors.ForbiddenAccess;
                        context.Response.StatusCode = 403;
                    }

                    context.Response.Headers.Remove("Auth-Fail-Type");

                    var localizedDescription = _localizer.GetString(error.Code);

                    var errorMessage = string.IsNullOrEmpty(localizedDescription) ? error.Description : localizedDescription;

                    ApiResponse response = ApiResponse.Failure(context, errorMessage, error.Code, context.Response.StatusCode);

                    response.Instance = context.Request.Path.Value!;

                    response.TraceId = context.TraceIdentifier;

                    await context.Response.WriteAsJsonAsync(response);








                }
            }

        }
    }
}
