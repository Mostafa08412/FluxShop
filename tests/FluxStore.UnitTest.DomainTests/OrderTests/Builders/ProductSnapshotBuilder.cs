using FluxStore.Api.Domain.Enums;
using FluxStore.Api.Domain.OrderAggregate.ValueObjects;

namespace FluxStore.UnitTest.DomainTests.OrderTests.Builders
{
    public class ProductSnapshotBuilder
    {
        public Guid ProductId { get; private set; } = Guid.Empty;
        public Guid ProductVariantId { get; private set; } = Guid.Empty;
        public string ProductName { get; private set; } = String.Empty;
        public string ProductVariantColor { get; private set; } = String.Empty;
        public ProductVariantSize ProductVariantSize { get; private set; } = ProductVariantSize.None;
        public decimal UnitPrice { get; private set; } = default;

        public static ProductSnapshotBuilder Create() => new();
        public ProductSnapshotBuilder WithValidData()
        {
            ProductId = Guid.CreateVersion7();
            ProductVariantId = Guid.CreateVersion7();
            ProductName = "Shirt";
            ProductVariantColor = "Red";
            ProductVariantSize = ProductVariantSize.XL;
            UnitPrice = 1200;

            return this;
        }

        public ProductSnapshotBuilder WithInValidData()
        {
            ProductId = Guid.Empty;
            ProductVariantId = Guid.Empty;
            ProductName = "";
            ProductVariantColor = "";
            ProductVariantSize = ProductVariantSize.None;
            UnitPrice = -1200;

            return this;
        }

        public OrderItemProductSnapshot? Build()
        {
            var snapShot = OrderItemProductSnapshot.Create(ProductId, ProductVariantId, ProductName, ProductVariantColor, ProductVariantSize, UnitPrice);

            if (!snapShot.IsSuccess)
                return null;

            return snapShot.Value;
        }


    }
}
