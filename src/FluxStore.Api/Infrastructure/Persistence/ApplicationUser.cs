using FluxStore.Api.Shared.Models;
using Microsoft.AspNetCore.Identity;

namespace FluxStore.Api.Infrastructure.Persistence
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public List<RefreshToken> RefreshTokens { get; set; } = new();


    }


}
