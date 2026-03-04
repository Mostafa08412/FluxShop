using FluxStore.Api.Shared.Abstractions;

namespace FluxStore.Api.Infrastructure.Services
{
    public class DateTimeProvider : IDateTime
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
