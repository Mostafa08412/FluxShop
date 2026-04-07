using FluxStore.Api.Domain.UserAggregate;
using FluxStore.Api.Domain.UserAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FluxStore.Api.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(x => x.DisplayName).HasMaxLength(100);
            builder.Property(x => x.AvatarUrl).HasMaxLength(2048);

            builder.OwnsOne(x => x.Phone, phone =>
            {
                phone.Property(x => x.Value).HasColumnName("Phone").HasMaxLength(16);
            });

            builder.Navigation(x => x.Phone).IsRequired(false);

            builder.OwnsOne(x => x.NotificationSettings, settings =>
            {
                settings.Property(x => x.OrderUpdatesEmail).HasColumnName("Notification_OrderUpdatesEmail");
                settings.Property(x => x.OrderUpdatesPush).HasColumnName("Notification_OrderUpdatesPush");
                settings.Property(x => x.PromotionsEmail).HasColumnName("Notification_PromotionsEmail");
                settings.Property(x => x.PromotionsPush).HasColumnName("Notification_PromotionsPush");
                settings.Property(x => x.RecommendationsEmail).HasColumnName("Notification_RecommendationsEmail");
                settings.Property(x => x.RecommendationsPush).HasColumnName("Notification_RecommendationsPush");
            });
        }
    }
}
