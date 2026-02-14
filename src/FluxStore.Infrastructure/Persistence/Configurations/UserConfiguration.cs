using FluxStore.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FluxStore.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {


            builder.HasKey(x => x.Id);

            builder.Property(X => X.FirstName).IsRequired();

            builder.Property(X => X.LastName).IsRequired();

            builder.Property(X => X.Id).IsRequired();

        }
    }
}
