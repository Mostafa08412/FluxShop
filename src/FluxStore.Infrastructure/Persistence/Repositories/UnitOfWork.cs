using FluxStore.Application.Common.Interfaces;
using FluxStore.Domain.Abstractions;
using FluxStore.Domain.Core.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace FluxStore.Infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {


        protected IDbContextTransaction _currentTransaction { get; private set; }

        protected ApplicationDbContext dbContext;

        private readonly ICurrentUser _currentUser;
        private readonly IDateTime _dateTime;
        private readonly IMediator _mediator;

        public UnitOfWork(ApplicationDbContext dbContext, ICurrentUser currentUser, IDateTime dateTime, IMediator mediator)
        {

            this.dbContext = dbContext;
            _currentUser = currentUser;
            _dateTime = dateTime;
            _mediator = mediator;
        }

        public async Task<int> Complete(CancellationToken cancellationToken)
        {
            AuditAddedAndModifiedEntries();

            var result = await dbContext.SaveChangesAsync(cancellationToken);

            while (true)
            {
                var aggregateEntitiesWithDomainEvents =
                   dbContext.ChangeTracker.Entries<Aggregate>()
                   .Select(X => X.Entity)
                   .Where(X => X.DomainEvents.Any()).ToList();

                if (!aggregateEntitiesWithDomainEvents.Any()) break;

                await PublishAggregatesDomainEvents(aggregateEntitiesWithDomainEvents, cancellationToken);
            }



            return await dbContext.SaveChangesAsync(cancellationToken);
        }


        private void AuditAddedAndModifiedEntries()
        {
            //Search for all added entities 
            var seedCreatedOrUpdatedBy = _currentUser.UserId ?? Guid.CreateVersion7().ToString();

            foreach (var entry in dbContext.ChangeTracker.Entries<IAuditable>())
            {

                entry.Property(X => X.UpdatedBy).CurrentValue =
                    entry.Entity.UpdatedBy == default ? seedCreatedOrUpdatedBy : entry.Entity.UpdatedBy;

                entry.Property(X => X.UpdatedOnUTC).CurrentValue =
                 entry.Entity.UpdatedOnUTC == default ? _dateTime.UTCNow : entry.Entity.UpdatedOnUTC;


                if (entry.State == EntityState.Added)
                {
                    entry.Property(X => X.CreatedBy).CurrentValue =
                        entry.Entity.CreatedBy == default ? seedCreatedOrUpdatedBy : entry.Entity.CreatedBy;

                    entry.Property(X => X.CreatedOnUTC).CurrentValue =
                       entry.Entity.CreatedOnUTC == default ? _dateTime.UTCNow : entry.Entity.CreatedOnUTC;


                }
            }
        }


        private async Task PublishAggregatesDomainEvents(List<Aggregate> aggregates, CancellationToken cancellationToken)
        {


            foreach (var entityWithDomainEvents in aggregates)
            {
                foreach (var domainEVent in entityWithDomainEvents.DomainEvents)
                {
                    await _mediator.Publish(domainEVent, cancellationToken);
                }
                entityWithDomainEvents.ClearDomainEvents();
            }

        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken)
        {

            _currentTransaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken)
        {
            await _currentTransaction.CommitAsync(cancellationToken);
        }

        public async Task RollBackAsync(CancellationToken cancellationToken)
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
        }
    }

}
