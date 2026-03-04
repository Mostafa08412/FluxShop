using Ardalis.Result;
using FluxStore.Api.Infrastructure.Persistence;
using FluxStore.Api.Shared.Abstractions;
using FluxStore.Api.Shared.Errors;
using FluxStore.Api.Shared.Models;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Hybrid;

namespace FluxStore.Api.Features.Authentication.ForgetPassword
{
    public class ForgetPasswordRequestHandler : IRequestHandler<ForgetPasswordRequest, Result<Unit>>
    {
        private readonly IBackgroundJobClient _backgroundJobClient;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly HybridCache _memoryCache;



        public ForgetPasswordRequestHandler(IBackgroundJobClient backgroundJobClient, UserManager<ApplicationUser> appUserManager, HybridCache memoryCache)
        {
            _backgroundJobClient = backgroundJobClient;
            _userManager = appUserManager;
            _memoryCache = memoryCache;
        }

        public async Task<Result<Unit>> Handle(ForgetPasswordRequest request, CancellationToken cancellationToken)
        {
            var cacheKey = $"ForgetPasswordCooldown_{request.EmailAddress}";

            bool isInCoolDown = await IsInCooldown(cacheKey, cancellationToken);

            if (isInCoolDown)
            {
                ErrorList errorList = new ErrorList(new[] { IdentityErrors.OtpCooldown.Code, IdentityErrors.OtpCooldown.Description });

                return Result<Unit>.Error(errorList);
            }

            var user = await _userManager.FindByEmailAsync(request.EmailAddress);

            if (user is null) return Result<Unit>.NoContent();

            await _userManager.UpdateSecurityStampAsync(user); // To invalidate existing tokens.

            var otp = await _userManager.GenerateTwoFactorTokenAsync(user, "ResetPasswordOTPProvider");

            var emailModel = new ForgetPasswordEmailModel
            {
                EmailAddress = user.Email!,
                Otp = otp,
                Name = $"{user.FirstName} {user.LastName}"
            };

            _backgroundJobClient.Enqueue<IEmailService>(u => u.SendEmail<ForgetPasswordEmailModel>(user.Email!, emailModel, cancellationToken));

            return Result<Unit>.Success(Unit.Value);

        }

        private async ValueTask<bool> IsInCooldown(string key, CancellationToken cancellationToken)
        {

            bool existingValue = await _memoryCache.GetOrCreateAsync<bool>
            (
                key,

                factory: async _ => false,

                cancellationToken: cancellationToken
            );

            if (!existingValue)
            {
                var hybridCacheOptions = new HybridCacheEntryOptions { Expiration = TimeSpan.FromSeconds(30) };

                await _memoryCache.SetAsync(key, true, hybridCacheOptions, cancellationToken: cancellationToken);
            }



            return existingValue; // Key existed, user must wait
        }
    }


}
