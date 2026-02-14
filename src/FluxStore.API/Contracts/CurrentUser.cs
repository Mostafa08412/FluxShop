using FluxStore.Application.Common.Interfaces;
using System.Security.Claims;

namespace FluxStore.Api.Contracts
{
    public sealed class CurrentUser : ICurrentUser
    {

        private readonly HttpContext httpContext;

        public CurrentUser(IHttpContextAccessor? httpContextAccessor)
        {

            this.httpContext = httpContextAccessor!.HttpContext!;
        }

        public string UserId
        {
            get
            {

                ClaimsPrincipal? user = httpContext?.User;

                bool IsAuthenticated = user?.Identity?.IsAuthenticated ?? false;

                if (!IsAuthenticated)
                    return "Unkown";

                else
                    return httpContext!.User.Claims.FirstOrDefault(X => X.Type == ClaimTypes.NameIdentifier)!.Value;

            }

        }


    }
}
