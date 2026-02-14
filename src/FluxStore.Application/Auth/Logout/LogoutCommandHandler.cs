using FluxStore.Application.Common.Interfaces;
using FluxStore.Domain.Core.Primitives.Result;
using MediatR;

namespace FluxStore.Application.Auth.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
    {
        private readonly ICurrentUser _currentUser;
        private readonly IIdentityService _identityService;

        public LogoutCommandHandler(ICurrentUser currentUser, IIdentityService identityService)
        {
            _currentUser = currentUser;
            _identityService = identityService;
        }

        public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            return await _identityService.RevokeActiveRefreshToken(_currentUser.UserId);
        }
    }
}
