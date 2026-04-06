using MediatR;
using System.Diagnostics;
using System.Text.Json;

namespace FluxStore.Api.Shared.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;

            _logger.LogInformation("Handling Request: {RequestName}", requestName);
            _logger.LogDebug("Request Content: {RequestContent}", JsonSerializer.Serialize(request));

            var stopwatch = Stopwatch.StartNew();

            try
            {
                var response = await next();

                stopwatch.Stop();

                _logger.LogInformation("Handled Request: {RequestName} in {ElapsedMilliseconds}ms",
                    requestName, stopwatch.ElapsedMilliseconds);
                
                _logger.LogDebug("Response Content: {ResponseContent}", JsonSerializer.Serialize(response));

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(ex, "Request {RequestName} failed after {ElapsedMilliseconds}ms with message {ErrorMessage}",
                    requestName, stopwatch.ElapsedMilliseconds, ex.Message);

                throw;
            }
        }
    }
}
