using FluxStore.Application.Common.Interfaces;

namespace FluxStore.Infrastructure.Common
{
    public class SettableDateProvider : IDateTime
    {
        public DateTime UTCNow { get; set; } = DateTime.UtcNow;

    }
}
