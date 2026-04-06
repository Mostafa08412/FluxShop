using Ardalis.Result;
using FluxStore.Api.Domain.OrderAggregate.ValueObjects;
using FluxStore.Api.Shared.Errors;
using FluxStore.Api.Shared.Extensions;

namespace FluxStore.Api.Domain.OrderAggregate
{
    public sealed class OrderItem : Entity
    {
        public OrderItemProductSnapshot ProductSnapshot { get; private set; }
        public int Quantity { get; private set; }

        private OrderItem(Guid id, OrderItemProductSnapshot productSnapshot, int quantity) : base(id)
        {
            ProductSnapshot = productSnapshot;
            Quantity = quantity;
        }

        private OrderItem() { }

        public static Result<OrderItem> Create(OrderItemProductSnapshot productSnapshot, int quantity)
        {

            if (productSnapshot is null)
                return Result<OrderItem>.Invalid(OrderErrors.ProductSnapshotIsRequired.ToValidationError());

            if (quantity < 1)
                return Result<OrderItem>.Invalid(OrderErrors.QuantityMustBeAtLeastOne.ToValidationError());

            return Result<OrderItem>.Success(
                new OrderItem(Guid.CreateVersion7(), productSnapshot!, quantity));
        }
    }
}
