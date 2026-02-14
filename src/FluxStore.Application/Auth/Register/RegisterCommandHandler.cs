using FluxStore.Domain.Core.Primitives.Result;
using MediatR;

namespace FluxStore.Application.Auth.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result>
    {
        private readonly IIdentityService _identityService;


        public RegisterCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var firstName = request.Name.Split(' ')[0];

            var lastName = request.Name.Split(' ').Length > 1 ? request.Name.Split(' ')[1] : string.Empty;

            var createUserResult = await _identityService.CreateUserAsync(firstName, lastName, request.EmailAddress, request.Password);


            return createUserResult;
        }
    }
}
