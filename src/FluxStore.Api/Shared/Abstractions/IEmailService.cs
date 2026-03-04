using Ardalis.Result;
using MediatR;

namespace FluxStore.Api.Shared.Abstractions
{
    public interface IEmailService
    {
        Task<Result<Unit>> SendBulkEmails<T>(string[] to, T emailModel, CancellationToken cancellationToken) where T : IEmailModel;
        Task<Result<Unit>> SendEmail<T>(string to, T emailModel, CancellationToken cancellationToken) where T : IEmailModel;
    }
}