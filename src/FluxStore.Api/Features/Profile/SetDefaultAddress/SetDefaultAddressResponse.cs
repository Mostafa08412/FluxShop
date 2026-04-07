namespace FluxStore.Api.Features.Profile.SetDefaultAddress
{
    public sealed record SetDefaultAddressResponse(
        Guid Id,
        string Label,
        string Street,
        string City,
        string? State,
        string Country,
        string PostalCode,
        bool IsDefault);
}
