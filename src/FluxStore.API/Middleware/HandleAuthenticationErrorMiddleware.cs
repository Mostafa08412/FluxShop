using FluxStore.Api.Infrastructure;
using FluxStore.Domain.Core.Errors;
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
            var helper = new ApiResponseHelper(context);


            await next.Invoke(context);

            if (context.Response.Headers.ContainsKey("Auth-Fail-Type"))
            {
                string headerValue = context.Response.Headers["Auth-Fail-Type"];

                if (!string.IsNullOrEmpty(headerValue))
                {
                    if (headerValue == Errors.IdentityErrors.InvalidToken.Code)
                    {
                        var error = Errors.IdentityErrors.InvalidToken;
                        context.Response.StatusCode = (int)ApplicationStatusCodes.Unauthorized;
                        await context.Response.WriteAsJsonAsync(helper.BasicErrorApiResponse(error.Description, error.Code));
                    }
                    if (headerValue == Errors.IdentityErrors.MissingToken.Code)
                    {
                        var error = Errors.IdentityErrors.MissingToken;
                        context.Response.StatusCode = (int)ApplicationStatusCodes.Unauthorized;
                        await context.Response.WriteAsJsonAsync(helper.BasicErrorApiResponse(error.Description, error.Code));
                    }

                    if (headerValue == Errors.IdentityErrors.ExpiredToken.Code)
                    {
                        var error = Errors.IdentityErrors.ExpiredToken;
                        context.Response.StatusCode = (int)ApplicationStatusCodes.Unauthorized;
                        await context.Response.WriteAsJsonAsync(helper.BasicErrorApiResponse(error.Description, error.Code));
                    }

                    if (headerValue == Errors.IdentityErrors.ForbiddenAccess.Code)
                    {
                        var error = Errors.IdentityErrors.ForbiddenAccess;
                        context.Response.StatusCode = (int)ApplicationStatusCodes.Forbidden;
                        await context.Response.WriteAsJsonAsync(helper.BasicErrorApiResponse(error.Description, error.Code));
                    }
                }
            }

        }
    }
}
