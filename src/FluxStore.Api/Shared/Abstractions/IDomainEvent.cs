using MediatR;

namespace FluxStore.Api.Shared.Abstractions
{
    public interface IDomainEvent : INotification
    {
    }
}
