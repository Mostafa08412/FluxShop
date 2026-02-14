using FluxStore.Application.Auth.Common;
using FluxStore.Domain.Core.Primitives.Result;
using FluxStore.Application.Auth.Common;
using FluxStore.Application.Contracts.Identity;
using MediatR;

namespace FluxStore.Application.Auth.Login
{

    public class LoginRequestHandler : IRequestHandler<LoginRequest, Result<AuthenticationResponse>>
    {
        private readonly IIdentityService _identityService;

        public LoginRequestHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<Result<AuthenticationResponse>> Handle(
         LoginRequest request,
         CancellationToken cancellationToken)
        {
            var result = await _identityService.AuthenticateAsync(
                request.EmailAddress,
                request.Password,
                cancellationToken);

            if (!result.IsSuccess)
                return Result<AuthenticationResponse>.Failure(result.Errors);


            var authData = result.Value!;

            var response = new AuthenticationResponse
            {
                User = new IdentityUserDto
                {
                    Id = authData.UserId,
                    Email = authData.Email,
                    FirstName = authData.FirstName,
                    LastName = authData.LastName,
                    UserName = authData.UserName,
                    Roles = authData.Roles
                },
                AccessToken = new AccessTokenDto
                {
                    Token = authData.AccessToken,
                    ExpiresAt = authData.AccessTokenExpiresAt.ToLocalTime()
                },
                RefreshToken = new RefreshTokenDto
                {
                    Token = authData.RefreshToken,
                    ExpiresAt = authData.RefreshTokenExpiresAt.ToLocalTime()
                }
            };

            return Result<AuthenticationResponse>.Success(response);
        }
    }

}

