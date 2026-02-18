using FluxStore.Application.Common.Interfaces;
using FluxStore.Domain.Users;
using FluxStore.Infrastructure.Persistence.Identity;
using FluxStore.Infrastructure.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Reflection;

namespace FluxStore.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>, IApplicationDbContext
    {
        public DbSet<User> BusinessUsers { get; private set; }
        public DbSet<RefreshToken> RefreshTokens { get; private set; }
        public DatabaseFacade DB { get; private set; }


        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            this.DB = this.Database;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
