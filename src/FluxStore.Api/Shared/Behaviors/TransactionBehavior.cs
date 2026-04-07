using FluxStore.Api.Infrastructure.Persistence;
using FluxStore.Api.Shared.Markers;
using MediatR;
namespace FluxStore.Api.Shared.Behaviors

{
    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull

    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

        public TransactionBehavior(ApplicationDbContext context, ILogger<TransactionBehavior<TRequest, TResponse>> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (request is ICommand)
            {

                using (var transaction = _context.Database.BeginTransaction())
                {

                    try
                    {
                        var response = await next();

                        var affectedRows = await _context.SaveChangesAsync(cancellationToken);

                        transaction.Commit();

                        _logger.LogInformation("Transaction committed successfully, {affectedRows} affected rows.", affectedRows);

                        return response;


                    }

                    catch
                    {
                        _logger.LogError("An error occurred during the transaction. Rolling back changes.");
                        _context.Database.RollbackTransaction();
                        throw;

                    }

                }
            }
            else
            {
                return await next();

            }
        }
    }
}
