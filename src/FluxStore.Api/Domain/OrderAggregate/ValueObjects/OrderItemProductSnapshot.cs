using Ardalis.Result;
using FluxStore.Api.Domain.Enums;
using FluxStore.Api.Shared.Errors;
using FluxStore.Api.Shared.Extensions;

namespace FluxStore.Api.Domain.OrderAggregate.ValueObjects
{
    public sealed class OrderItemProductSnapshot : ValueObject
    {
        public Guid ProductId { get; }
        public Guid ProductVariantId { get; }
        public string ProductName { get; }
        public string ProductVariantColor { get; }
        public ProductVariantSize ProductVariantSize { get; }
        public decimal UnitPrice { get; }

        private OrderItemProductSnapshot(
            Guid productId, Guid productVariantId,
            string productName, string productVariantColor,
            ProductVariantSize productVariantSize, decimal unitPrice)
        {
            ProductId = productId;
            ProductVariantId = productVariantId;
            ProductName = productName;
            ProductVariantColor = productVariantColor;
            ProductVariantSize = productVariantSize;
            UnitPrice = unitPrice;
        }

        public static Result<OrderItemProductSnapshot> Create(
            Guid productId, Guid productVariantId,
            string productName, string productVariantColor,
            ProductVariantSize productVariantSize, decimal unitPrice)
        {
            if (productId == Guid.Empty)
                return Result<OrderItemProductSnapshot>.Invalid(OrderErrors.ProductIdIsRequired.ToValidationError());
            if (productVariantId == Guid.Empty)
                return Result<OrderItemProductSnapshot>.Invalid(OrderErrors.ProductVariantIdIsRequired.ToValidationError());
            if (string.IsNullOrWhiteSpace(productName))
                return Result<OrderItemProductSnapshot>.Invalid(OrderErrors.ProductNameIsRequired.ToValidationError());
            if (string.IsNullOrWhiteSpace(productVariantColor))
                return Result<OrderItemProductSnapshot>.Invalid(OrderErrors.ProductVariantColorIsRequired.ToValidationError());
            if (!Enum.IsDefined(productVariantSize) || productVariantSize == ProductVariantSize.None)
                return Result<OrderItemProductSnapshot>.Invalid(OrderErrors.InvalidProductVariantSize.ToValidationError());
            if (unitPrice <= 0)
                return Result<OrderItemProductSnapshot>.Invalid(OrderErrors.UnitPriceMustBePositive.ToValidationError());

            return Result<OrderItemProductSnapshot>.Success(
                new OrderItemProductSnapshot(productId, productVariantId,
                    productName, productVariantColor,
                    productVariantSize, unitPrice));
        }

        public override IEnumerable<object> GetAtomicValues()
        {
            yield return ProductId;
            yield return ProductVariantId;
            yield return ProductName;
            yield return ProductVariantColor;
            yield return ProductVariantSize;
            yield return UnitPrice;
        }
    }
}
