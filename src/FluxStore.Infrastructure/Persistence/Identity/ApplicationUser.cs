using AutoMapper;
using FluxStore.Application.Contracts.Identity;
using FluxStore.Domain.Abstractions;
using FluxStore.Infrastructure.Tokens;
using Microsoft.AspNetCore.Identity;

namespace FluxStore.Infrastructure.Persistence.Identity
{
    public class ApplicationUser : IdentityUser, IUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public List<RefreshToken> RefreshTokens { get; set; } = new();
    }

    public class ApplicationUserMappingProfile : Profile
    {
        public ApplicationUserMappingProfile()
        {
            CreateMap<ApplicationUser, IdentityUserDto>();

        }
    }
}
