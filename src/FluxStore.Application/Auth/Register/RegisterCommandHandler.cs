using FluxStore.Application.Auth.Common;
using FluxStore.Application.Contracts.Identity;
using FluxStore.Domain.Abstractions;
using FluxStore.Domain.Core.Primitives.Result;
using FluxStore.Domain.Users;
using MediatR;

namespace FluxStore.Application.Auth.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthenticationResponse>>
    {
        private readonly IIdentityService _identityService;

        private readonly IUnitOfWork _unitOfWork;

        public RegisterCommandHandler(IIdentityService identityService, IUnitOfWork unitOfWork)
        {
            _identityService = identityService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AuthenticationResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {

            var firstName = request.Name.Trim().Split(' ').FirstOrDefault() ?? string.Empty;
            var lastName = request.Name.Trim().Split(' ').Skip(1).FirstOrDefault() ?? string.Empty;

            var createUserResult = await _identityService.CreateUserAndAuthenticateAsync(firstName, lastName, request.EmailAddress, request.Password, cancellationToken);

            if (createUserResult.IsFailure)
                return Result<AuthenticationResponse>.Failure(createUserResult.Errors);

            var authData = createUserResult.Value!;

            var createDomainUserResult = User.Create(authData.UserId, authData.FirstName, authData.LastName, authData.Email, authData.Email);

            if (createDomainUserResult.IsFailure)
                return Result<AuthenticationResponse>.Failure(createDomainUserResult.Errors);

            _unitOfWork.Users.Add(createDomainUserResult.Value!);

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
