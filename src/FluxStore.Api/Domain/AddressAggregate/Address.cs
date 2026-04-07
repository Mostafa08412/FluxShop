using Ardalis.Result;
using FluxStore.Api.Domain.AddressAggregate.Events;
using FluxStore.Api.Domain.AddressAggregate.ValueObjects;
using FluxStore.Api.Shared.Abstractions;
using FluxStore.Api.Shared.Errors;
using FluxStore.Api.Shared.Extensions;

namespace FluxStore.Api.Domain.AddressAggregate
{
    public sealed class Address : Aggregate
    {
        public Guid UserId { get; private set; }
        public AddressLabel Label { get; private set; } = null!;
        public Street Street { get; private set; } = null!;
        public string City { get; private set; } = string.Empty;
        public string? State { get; private set; }
        public Country Country { get; private set; } = null!;
        public PostalCode PostalCode { get; private set; } = null!;
        public bool IsDefault { get; private set; }
        public bool IsDeleted { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset UpdatedAt { get; private set; }

        private Address() { }

        private Address(
            Guid id,
            Guid userId,
            AddressLabel label,
            Street street,
            string city,
            string? state,
            Country country,
            PostalCode postalCode,
            bool isDefault,
            DateTimeOffset occurredOn) : base(id)
        {
            UserId = userId;
            Label = label;
            Street = street;
            City = city;
            State = state;
            Country = country;
            PostalCode = postalCode;
            IsDefault = isDefault;
            CreatedAt = occurredOn;
            UpdatedAt = occurredOn;
        }

        public static Result<Address> Create(
            Guid id,
            Guid userId,
            AddressLabel label,
            Street street,
            string city,
            string? state,
            Country country,
            PostalCode postalCode,
            bool isDefault = false,
            DateTimeOffset? occurredOn = null)
        {
            var errors = new List<Error>();

            if (id == Guid.Empty)
            {
                errors.Add(AddressErrors.IdRequired);
            }

            if (userId == Guid.Empty)
            {
                errors.Add(AddressErrors.UserIdRequired);
            }

            if (label is null) errors.Add(AddressErrors.LabelRequired);
            if (street is null) errors.Add(AddressErrors.StreetRequired);
            if (country is null) errors.Add(AddressErrors.CountryRequired);
            if (postalCode is null) errors.Add(AddressErrors.PostalCodeRequired);

            if (string.IsNullOrWhiteSpace(city))
            {
                errors.Add(AddressErrors.CityRequired);
            }
            else if (city.Length > 100)
            {
                errors.Add(AddressErrors.CityTooLong);
            }

            if (!string.IsNullOrWhiteSpace(state) && state.Length > 100)
            {
                errors.Add(AddressErrors.StateTooLong);
            }

            if (errors.Any())
            {
                return Result<Address>.Invalid(errors.Select(x => x.ToValidationError()));
            }

            var now = occurredOn ?? DateTimeOffset.UtcNow;
            var address = new Address(id, userId, label!, street!, city.Trim(), state?.Trim(), country!, postalCode!, isDefault, now);
            address.RaiseDomainEvent(new AddressAdded(id, userId, label!, street!, city.Trim(), state?.Trim(), country!, postalCode!, isDefault));
            return Result<Address>.Success(address);
        }

        public Result Update(AddressLabel label, Street street, string city, string? state, Country country, PostalCode postalCode)
        {
            if (IsDeleted)
            {
                return Result.Invalid(AddressErrors.NotFound.ToValidationError());
            }

            var errors = new List<Error>();
            if (label is null) errors.Add(AddressErrors.LabelRequired);
            if (street is null) errors.Add(AddressErrors.StreetRequired);
            if (country is null) errors.Add(AddressErrors.CountryRequired);
            if (postalCode is null) errors.Add(AddressErrors.PostalCodeRequired);

            if (string.IsNullOrWhiteSpace(city))
            {
                errors.Add(AddressErrors.CityRequired);
            }

            if (errors.Any())
            {
                return Result.Invalid(errors.Select(x => x.ToValidationError()));
            }

            Label = label;
            Street = street;
            City = city.Trim();
            State = state?.Trim();
            Country = country;
            PostalCode = postalCode;
            UpdatedAt = DateTimeOffset.UtcNow;

            RaiseDomainEvent(new AddressUpdated(Id, label, street, City, State, country, postalCode));
            return Result.Success();
        }

        public Result Delete()
        {
            if (IsDeleted)
            {
                return Result.Success();
            }

            IsDeleted = true;
            UpdatedAt = DateTimeOffset.UtcNow;
            RaiseDomainEvent(new AddressDeleted(Id));
            return Result.Success();
        }

        public Result SetAsDefault()
        {
            if (IsDeleted)
            {
                return Result.Invalid(AddressErrors.CannotSetDeletedAsDefault.ToValidationError());
            }

            IsDefault = true;
            UpdatedAt = DateTimeOffset.UtcNow;
            RaiseDomainEvent(new DefaultAddressChanged(Id, UserId));
            return Result.Success();
        }

        public Result DemoteDefault()
        {
            IsDefault = false;
            UpdatedAt = DateTimeOffset.UtcNow;
            return Result.Success();
        }

        public void Apply(IDomainEvent domainEvent)
        {
            switch (domainEvent)
            {
                case AddressAdded added:
                    UserId = added.UserId;
                    Label = added.Label;
                    Street = added.Street;
                    City = added.City;
                    State = added.State;
                    Country = added.Country;
                    PostalCode = added.PostalCode;
                    IsDefault = added.IsDefault;
                    CreatedAt = added.OccurredOn;
                    UpdatedAt = added.OccurredOn;
                    break;
                case AddressUpdated updated:
                    Label = updated.Label;
                    Street = updated.Street;
                    City = updated.City;
                    State = updated.State;
                    Country = updated.Country;
                    PostalCode = updated.PostalCode;
                    UpdatedAt = updated.OccurredOn;
                    break;
                case AddressDeleted deleted:
                    IsDeleted = true;
                    UpdatedAt = deleted.OccurredOn;
                    break;
                case DefaultAddressChanged changed:
                    IsDefault = true;
                    UpdatedAt = changed.OccurredOn;
                    break;
            }
        }
    }
}
