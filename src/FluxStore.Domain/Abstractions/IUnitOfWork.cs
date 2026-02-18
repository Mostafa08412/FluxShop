

using FluxStore.Domain.Users;

namespace FluxStore.Domain.Abstractions
{
    public interface IUnitOfWork
    {
        public IUserRepository Users { get; }
        public Task<int> Complete(CancellationToken cancellationToken);

        public Task BeginTransactionAsync(CancellationToken cancellationToken);

        public Task CommitTransactionAsync(CancellationToken cancellationToken);

        public Task RollBackAsync(CancellationToken cancellationToken);
    }
}
