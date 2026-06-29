using FluxStore.Api.Domain.PaymentMethodAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FluxStore.Api.Infrastructure.Persistence.Configurations
{
    public sealed class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
    {
        public void Configure(EntityTypeBuilder<PaymentMethod> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.Brand).IsRequired();
            builder.Property(x => x.ExpiryMonth).IsRequired();
            builder.Property(x => x.ExpiryYear).IsRequired();
            builder.Property(x => x.IsDefault).IsRequired();
            builder.Property(x => x.IsDeleted).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();

            builder.OwnsOne(x => x.Last4, last4 =>
            {
                last4.Property(x => x.Value)
                    .HasColumnName("Last4")
                    .HasMaxLength(4)
                    .IsRequired();
            });

            builder.OwnsOne(x => x.GatewayToken, gatewayToken =>
            {
                gatewayToken.Property(x => x.Value)
                    .HasColumnName("GatewayToken")
                    .HasMaxLength(512)
                    .IsRequired();
            });

            builder.Navigation(x => x.Last4).IsRequired();
            builder.Navigation(x => x.GatewayToken).IsRequired();

            builder.HasQueryFilter(X => !X.IsDeleted);

            builder.HasIndex(x => new { x.UserId, x.IsDeleted });
        }
    }
}
