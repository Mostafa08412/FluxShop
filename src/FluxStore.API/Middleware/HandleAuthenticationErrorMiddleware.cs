using FluxStore.Api.Infrastructure;
using FluxStore.Application.Common.Errors;
using FluxStore.Application.Common.Resources;
using FluxStore.Domain.Core.Primitives;
using Microsoft.Extensions.Localization;
namespace FluxStore.Api.Middleware
{
    public class HandleAuthenticationErrorMiddleware
    {
        private RequestDelegate next;
        private IStringLocalizer<SharedResource> _localizer;
        public HandleAuthenticationErrorMiddleware(RequestDelegate next, IStringLocalizer<SharedResource> localizer)
        {
            this.next = next;
            _localizer = localizer;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var helper = new ApiResponseHelper(_localizer);


            await next.Invoke(context);

            if (context.Response.Headers.ContainsKey("Auth-Fail-Type"))
            {
                string? headerValue = context.Response.Headers["Auth-Fail-Type"];

                if (!string.IsNullOrEmpty(headerValue))
                {
                    Error error = ApplicationErrors.IdentityErrors.InvalidToken;

                    if (headerValue == ApplicationErrors.IdentityErrors.InvalidToken.Code)
                    {
                        error = ApplicationErrors.IdentityErrors.InvalidToken;
                        context.Response.StatusCode = (int)ApplicationStatusCodes.Unauthorized;
                    }
                    if (headerValue == ApplicationErrors.IdentityErrors.MissingToken.Code)
                    {
                        error = ApplicationErrors.IdentityErrors.MissingToken;
                        context.Response.StatusCode = (int)ApplicationStatusCodes.Unauthorized;
                    }

                    if (headerValue == ApplicationErrors.IdentityErrors.ExpiredToken.Code)
                    {
                        error = ApplicationErrors.IdentityErrors.ExpiredToken;
                        context.Response.StatusCode = (int)ApplicationStatusCodes.Unauthorized;
                    }

                    if (headerValue == ApplicationErrors.IdentityErrors.ForbiddenAccess.Code)
                    {
                        error = ApplicationErrors.IdentityErrors.ForbiddenAccess;
                        context.Response.StatusCode = (int)ApplicationStatusCodes.Forbidden;
                    }
                    await context.Response.WriteAsJsonAsync(helper.BasicErrorApiResponse(error.Description, error.Code, context.Request.Path.Value ?? "Unkown", context.TraceIdentifier));

                }
            }

        }
    }
}
