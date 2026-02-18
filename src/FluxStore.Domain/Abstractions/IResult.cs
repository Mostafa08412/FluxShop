using FluxStore.Domain.Core.Primitives;
using FluxStore.Domain.Core.Primitives.Result;

namespace FluxStore.Domain.Abstractions;

public interface IResult
{
    Error? Error { get; }
    IEnumerable<Error> Errors { get; }
    bool IsFailure { get; }
    bool IsSuccess { get; }
    string Message { get; init; }

    static abstract Result Failure(Error error);
    static abstract Result Failure(IEnumerable<Error> errors);
    static abstract Result Success();
    static abstract Result Success(string Message);
}
