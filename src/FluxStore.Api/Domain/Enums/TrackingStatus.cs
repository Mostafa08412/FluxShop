namespace FluxStore.Api.Domain.Enums
{
    public enum TrackingStatus
    {
        OrderPlaced,
        Confirmed,
        Processing,
        Shipped,
        OutForDelivery,
        Delivered,
        Cancelled,
        Refunded
    }
}
