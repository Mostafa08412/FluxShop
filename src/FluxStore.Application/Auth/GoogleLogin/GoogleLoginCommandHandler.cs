using FluxStore.Application.Auth.Common;
using FluxStore.Application.Contracts.Identity;
using FluxStore.Domain.Core.Errors;
using FluxStore.Domain.Core.Primitives.Result;
using MediatR;
using System.Security.Cryptography;

namespace FluxStore.Application.Auth.GoogleLogin
{
    public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommand, Result<AuthenticationResponse>>
    {
        private readonly IIdentityService _identityService;


        public GoogleLoginCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<Result<AuthenticationResponse>> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
        {
            var result = await _identityService.AuthenticateWithGoolge(request.IdToken, cancellationToken);

            if (!result.IsSuccess)
                return Result<AuthenticationResponse>.Failure(result.Errors);

            var authData = result.Value!;


            var authenticationResponseResult = await _identityService.AuthenticateUsingEmailOnlyAsync(authData.emailAddress, cancellationToken);

            if (authenticationResponseResult.IsFailure && authenticationResponseResult.Errors.Any(X => X.Code == Errors.IdentityErrors.UserNotFoundByEmail(authData.emailAddress).Code))
            {
                var firstName = authData.fullName.Split(' ')[0];

                var lastName = authData.fullName.Split(' ')[1];

                var email = authData.emailAddress;

                var randomPassword = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

                var newUser = await _identityService.CreateUserAsync(firstName, lastName, email, randomPassword, cancellationToken);



                if (newUser.IsSuccess)
                {
                    authenticationResponseResult = await _identityService.AuthenticateUsingEmailOnlyAsync(email, cancellationToken);

                    if (authenticationResponseResult.IsFailure)
                        return Result<AuthenticationResponse>.Failure(authenticationResponseResult.Errors);
                }


            }

            var AuthenticationResponse = authenticationResponseResult.Value!;

            var response = new AuthenticationResponse
            {
                User = new IdentityUserDto
                {
                    Id = AuthenticationResponse.UserId,
                    Email = AuthenticationResponse.Email,
                    FirstName = AuthenticationResponse.FirstName,
                    LastName = AuthenticationResponse.LastName,
                    UserName = AuthenticationResponse.UserName,
                    Roles = AuthenticationResponse.Roles
                },
                AccessToken = new AccessTokenDto
                {
                    Token = AuthenticationResponse.AccessToken,
                    ExpiresAt = AuthenticationResponse.AccessTokenExpiresAt.ToLocalTime()
                },
                RefreshToken = new RefreshTokenDto
                {
                    Token = AuthenticationResponse.RefreshToken,
                    ExpiresAt = AuthenticationResponse.RefreshTokenExpiresAt.ToLocalTime()
                }
            };



            return Result<AuthenticationResponse>.Success(response);
        }
    }
}
