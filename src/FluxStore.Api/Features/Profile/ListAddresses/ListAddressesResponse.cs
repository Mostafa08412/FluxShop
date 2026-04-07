namespace FluxStore.Api.Features.Profile.ListAddresses
{
    public sealed record ListAddressesResponse(List<AddressDto> Addresses);

    public sealed record AddressDto(
        Guid Id,
        string Label,
        string Street,
        string City,
        string? State,
        string Country,
        string PostalCode,
        bool IsDefault);
}
