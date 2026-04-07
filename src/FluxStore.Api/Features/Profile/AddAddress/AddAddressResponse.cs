namespace FluxStore.Api.Features.Profile.AddAddress
{
    public sealed record AddAddressResponse(
        Guid Id,
        string Label,
        string Street,
        string City,
        string? State,
        string Country,
        string PostalCode,
        bool IsDefault);
}
