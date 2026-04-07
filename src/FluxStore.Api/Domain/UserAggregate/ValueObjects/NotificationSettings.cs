using FluxStore.Api.Domain;

namespace FluxStore.Api.Domain.UserAggregate.ValueObjects
{
    public sealed class NotificationSettings : ValueObject
    {
        public bool OrderUpdatesEmail { get; }
        public bool OrderUpdatesPush { get; }
        public bool PromotionsEmail { get; }
        public bool PromotionsPush { get; }
        public bool RecommendationsEmail { get; }
        public bool RecommendationsPush { get; }

        public static NotificationSettings Default => new(
            orderUpdatesEmail: true,
            orderUpdatesPush: true,
            promotionsEmail: false,
            promotionsPush: false,
            recommendationsEmail: false,
            recommendationsPush: false);

        public NotificationSettings(
            bool orderUpdatesEmail,
            bool orderUpdatesPush,
            bool promotionsEmail,
            bool promotionsPush,
            bool recommendationsEmail,
            bool recommendationsPush)
        {
            OrderUpdatesEmail = orderUpdatesEmail;
            OrderUpdatesPush = orderUpdatesPush;
            PromotionsEmail = promotionsEmail;
            PromotionsPush = promotionsPush;
            RecommendationsEmail = recommendationsEmail;
            RecommendationsPush = recommendationsPush;
        }

        public override IEnumerable<object> GetAtomicValues()
        {
            yield return OrderUpdatesEmail;
            yield return OrderUpdatesPush;
            yield return PromotionsEmail;
            yield return PromotionsPush;
            yield return RecommendationsEmail;
            yield return RecommendationsPush;
        }
    }
}
