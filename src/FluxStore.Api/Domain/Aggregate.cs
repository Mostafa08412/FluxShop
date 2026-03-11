using FluxStore.Api.Shared.Abstractions;

namespace FluxStore.Api.Domain
{
    public abstract class Aggregate : Entity
    {
        protected Aggregate() : base() { }

        public Aggregate(Guid Id) : base(Id) { }

        private readonly List<IDomainEvent> _domainEvents = new();

        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents;


        public void RaiseDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }



    }


}
