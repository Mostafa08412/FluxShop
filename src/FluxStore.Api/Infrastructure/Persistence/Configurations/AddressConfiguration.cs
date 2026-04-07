using FluxStore.Api.Domain.AddressAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FluxStore.Api.Infrastructure.Persistence.Configurations
{
    public sealed class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.City).HasMaxLength(100).IsRequired();
            builder.Property(x => x.State).HasMaxLength(100);
            builder.Property(x => x.IsDefault).IsRequired();
            builder.Property(x => x.IsDeleted).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();

            builder.OwnsOne(x => x.Label, label =>
            {
                label.Property(x => x.Value)
                    .HasColumnName("Label")
                    .HasMaxLength(50)
                    .IsRequired();
            });

            builder.OwnsOne(x => x.Street, street =>
            {
                street.Property(x => x.Value)
                    .HasColumnName("Street")
                    .HasMaxLength(200)
                    .IsRequired();
            });

            builder.OwnsOne(x => x.Country, country =>
            {
                country.Property(x => x.Name)
                    .HasColumnName("Country")
                    .HasMaxLength(100)
                    .IsRequired();
            });

            builder.OwnsOne(x => x.PostalCode, postalCode =>
            {
                postalCode.Property(x => x.Value)
                    .HasColumnName("PostalCode")
                    .HasMaxLength(20)
                    .IsRequired();
            });

            builder.Navigation(x => x.Label).IsRequired();
            builder.Navigation(x => x.Street).IsRequired();
            builder.Navigation(x => x.Country).IsRequired();
            builder.Navigation(x => x.PostalCode).IsRequired();

            builder.HasQueryFilter(x => !x.IsDeleted);

            builder.HasIndex(x => new { x.UserId, x.IsDeleted });

        }
    }
}
