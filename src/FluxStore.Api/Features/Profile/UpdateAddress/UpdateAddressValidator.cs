using FluentValidation;
using FluxStore.Api.Shared.Errors;

namespace FluxStore.Api.Features.Profile.UpdateAddress
{
    public sealed class UpdateAddressValidator : AbstractValidator<UpdateAddressRequest>
    {
        public UpdateAddressValidator()
        {
            RuleFor(x => x.Id).NotEmpty();

            RuleFor(x => x.Label)
                .NotEmpty().WithMessage(AddressErrors.LabelRequired.Description)
                .WithErrorCode(AddressErrors.LabelRequired.Code)
                .WithName(nameof(UpdateAddressRequest.Label))
                .MaximumLength(50).WithMessage(AddressErrors.LabelTooLong.Description).WithErrorCode(AddressErrors.LabelTooLong.Code);


            RuleFor(x => x.Street)
                .NotEmpty().WithMessage(AddressErrors.StreetRequired.Description).WithErrorCode(AddressErrors.StreetRequired.Code)
                .MaximumLength(200).WithMessage(AddressErrors.StreetTooLong.Description).WithErrorCode(AddressErrors.StreetTooLong.Code);

            RuleFor(x => x.City)
                .NotEmpty().WithMessage(AddressErrors.CityRequired.Description).WithErrorCode(AddressErrors.CityRequired.Code)
                .MaximumLength(100).WithMessage(AddressErrors.CityTooLong.Description).WithErrorCode(AddressErrors.CityTooLong.Code);

            RuleFor(x => x.State)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.State))
                .WithMessage(AddressErrors.StateTooLong.Description)
                .WithErrorCode(AddressErrors.StateTooLong.Code);

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage(AddressErrors.CountryRequired.Description).WithErrorCode(AddressErrors.CountryRequired.Code)
                .MaximumLength(100).WithMessage(AddressErrors.CountryTooLong.Description).WithErrorCode(AddressErrors.CountryTooLong.Code);

            RuleFor(x => x.PostalCode)
                .NotEmpty().WithMessage(AddressErrors.PostalCodeRequired.Description).WithErrorCode(AddressErrors.PostalCodeRequired.Code)
                .MaximumLength(20).WithMessage(AddressErrors.PostalCodeTooLong.Description).WithErrorCode(AddressErrors.PostalCodeTooLong.Code);
        }
    }
}
