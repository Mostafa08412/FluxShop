using FluxStore.Api.Infrastructure;
using FluxStore.Domain.Core.Errors;
using FluxStore.Domain.Core.Primitives;
namespace FluxStore.Api.Middleware
{
    public class HandleAuthenticationErrorMiddleware
    {
        private RequestDelegate next;

        public HandleAuthenticationErrorMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var helper = new ApiResponseHelper();


            await next.Invoke(context);

            if (context.Response.Headers.ContainsKey("Auth-Fail-Type"))
            {
                string? headerValue = context.Response.Headers["Auth-Fail-Type"];

                if (!string.IsNullOrEmpty(headerValue))
                {
                    Error error = Errors.IdentityErrors.InvalidToken;

                    if (headerValue == Errors.IdentityErrors.InvalidToken.Code)
                    {
                        error = Errors.IdentityErrors.InvalidToken;
                        context.Response.StatusCode = (int)ApplicationStatusCodes.Unauthorized;
                    }
                    if (headerValue == Errors.IdentityErrors.MissingToken.Code)
                    {
                        error = Errors.IdentityErrors.MissingToken;
                        context.Response.StatusCode = (int)ApplicationStatusCodes.Unauthorized;
                    }

                    if (headerValue == Errors.IdentityErrors.ExpiredToken.Code)
                    {
                        error = Errors.IdentityErrors.ExpiredToken;
                        context.Response.StatusCode = (int)ApplicationStatusCodes.Unauthorized;
                    }

                    if (headerValue == Errors.IdentityErrors.ForbiddenAccess.Code)
                    {
                        error = Errors.IdentityErrors.ForbiddenAccess;
                        context.Response.StatusCode = (int)ApplicationStatusCodes.Forbidden;
                    }
                    await context.Response.WriteAsJsonAsync(helper.BasicErrorApiResponse(error.Description, error.Code, context.Request.Path.Value ?? "Unkown", context.TraceIdentifier));

                }
            }

        }
    }
}
