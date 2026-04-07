using FluentValidation;
using FluxStore.Api.Shared.Errors;

namespace FluxStore.Api.Features.Profile.UpdateProfile
{
    public sealed class UpdateProfileValidator : AbstractValidator<UpdateProfileRequest>
    {
        public UpdateProfileValidator()
        {
            RuleFor(x => x.DisplayName)
                .NotEmpty()
                    .WithMessage(ProfileErrors.DisplayNameRequired.Description)
                    .WithErrorCode(ProfileErrors.DisplayNameRequired.Code)
                .MaximumLength(100)
                    .WithMessage(ProfileErrors.DisplayNameTooLong.Description)
                    .WithErrorCode(ProfileErrors.DisplayNameTooLong.Code);

            RuleFor(x => x.Phone)
                .Matches(@"^\+[1-9]\d{1,14}$")
                    .When(x => !string.IsNullOrWhiteSpace(x.Phone))
                    .WithMessage(ProfileErrors.InvalidPhone.Description)
                    .WithErrorCode(ProfileErrors.InvalidPhone.Code);

            RuleFor(x => x.AvatarUrl)
                .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
                    .When(x => !string.IsNullOrWhiteSpace(x.AvatarUrl))
                    .WithMessage("Avatar URL must be a valid absolute URL.");
        }
    }
}
