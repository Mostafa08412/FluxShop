namespace FluxStore.Api.Features.Profile.UpdateAddress
{
    public sealed record UpdateAddressResponse(
        Guid Id,
        string Label,
        string Street,
        string City,
        string? State,
        string Country,
        string PostalCode,
        bool IsDefault);
}
