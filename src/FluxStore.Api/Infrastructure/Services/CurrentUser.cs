using System.Security.Claims;

namespace FluxStore.Api.Infrastructure.Services
{
    public class CurrentUser
    {

        private readonly HttpContext? httpContext;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            httpContext = httpContextAccessor?.HttpContext;
        }

        public Guid UserId =>
            httpContext?
            .User?
            .Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value is string id ? Guid.Parse(id) : Guid.Empty;

        public string? Email =>
            httpContext?
            .User?
            .Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        public string? FirstName =>
            httpContext?
            .User?
            .Claims
            .FirstOrDefault(c => c.Type == "FirstName")?.Value;

        public string? Lastname =>
            httpContext?
            .User?
            .Claims
            .FirstOrDefault(c => c.Type == "Lastname")?.Value;
    }
}
