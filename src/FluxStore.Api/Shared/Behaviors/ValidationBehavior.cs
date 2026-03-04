using Ardalis.Result;
using FluentValidation;
using FluxStore.Api.Shared.Extensions;
using MediatR;
namespace FluxStore.Api.Shared.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : Ardalis.Result.IResult
    {


        private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;
        private readonly IEnumerable<IValidator<TRequest>> _validators; // injected validators for the request

        public ValidationBehavior(ILogger<ValidationBehavior<TRequest, TResponse>> logger, IEnumerable<IValidator<TRequest>> validators)
        {
            _logger = logger;
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {

            if (_validators != null || _validators!.Any()) // check if the request has any validators
            {

                ValidationContext<TRequest> context = new ValidationContext<TRequest>(request);

                var failures = _validators!
                                .Select(validator => validator.Validate(context))
                                .SelectMany(validationResult => validationResult.Errors)
                                .Where(validationFailure => validationFailure != null) // null validation failures means validation passed, so we only want to consider non-null failures
                                .ToList();

                IEnumerable<ValidationError> validationErrors = failures.Select(X => X.ToValidationError());
                if (validationErrors.Any())
                {
                    _logger.LogWarning("Validation failed for request {RequestType}", typeof(TRequest).Name);

                    var valueType = typeof(TResponse).GenericTypeArguments[0];

                    var resultType = typeof(Result<>).MakeGenericType(valueType);

                    var invalidMethod = resultType.GetMethod(
                        nameof(Result.Invalid),
                        new[] { typeof(IEnumerable<ValidationError>) });

                    var result = invalidMethod!.Invoke(null, new object[] { validationErrors });

                    return (TResponse)result!;
                }

            }



            return await next();

        }


    }
}
