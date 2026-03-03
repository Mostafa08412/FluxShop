using FluxStore.Application.Common.Errors;
using FluxStore.Application.Common.Interfaces;
using FluxStore.Application.Contracts.Identity;
using FluxStore.Application.Users.Queries.GetUser;
using FluxStore.Domain.Core.Primitives.Result;
using FluxStore.Domain.Enums;
using FluxStore.Infrastructure.Extensions;
using FluxStore.Infrastructure.Persistence;
using FluxStore.Infrastructure.Persistence.Identity;
using FluxStore.Infrastructure.Tokens;
using FluxStore.Infrastructure.Tokens.Options;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using RefreshToken = FluxStore.Infrastructure.Tokens.RefreshToken;
namespace FluxStore.Infrastructure.Authentication
{


    public sealed class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly TokenSettings _tokenOptions;
        private readonly IDateTime _dateTime;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;

        public IdentityService(IMemoryCache cache, IConfiguration configuration, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context, ITokenService tokenService, IOptions<TokenSettings> tokenSettings, IDateTime dateTime)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = context;
            _tokenService = tokenService;
            _tokenOptions = tokenSettings.Value;
            _dateTime = dateTime;
            _configuration = configuration;
            _cache = cache;
        }


        #region Private Helper Methods
        private static IdentityUserDto MapToDto(ApplicationUser user, IEnumerable<string> roles)
        {
            return new IdentityUserDto
            {
                Id = user.Id,
                Email = user.Email!,
                UserName = user.UserName!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = roles
            };
        }
        private async Task<Result<ApplicationUser>> ValidateUserByIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if ((user is null))
            {
                return Result<ApplicationUser>.Failure(ApplicationErrors.IdentityErrors.UserNotFoundById(userId.ToString()));
            }
            return Result<ApplicationUser>.Success(user);
        }

        private async Task<Result<ApplicationUser>> ValidateUserByEmail(string email, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if ((user is null))
            {
                return Result<ApplicationUser>.Failure(ApplicationErrors.IdentityErrors.UserNotFoundByEmail);
            }
            return Result<ApplicationUser>.Success(user);
        }
        #endregion

        #region User Roles Management Methods    
        public async Task<Result> AddRoleToUserAsync(Guid userId, string role, CancellationToken cancellationToken)
        {
            var userResult = await ValidateUserByIdAsync(userId, cancellationToken);
            if (!userResult.IsSuccess)
                return Result.Failure(userResult.Errors);

            var roleExists = await _roleManager.RoleExistsAsync(role);
            if (!roleExists)
                return Result.Failure(ApplicationErrors.IdentityErrors.RoleNotFound(role));

            var identityResult = await _userManager.AddToRoleAsync(userResult.Value!, role);
            if (!identityResult.Succeeded)
                return identityResult.ToResult();

            return Result.Success();
        }
        public async Task<Result> AddRolesToUserAsync(Guid userId, IEnumerable<string> roles, CancellationToken cancellationToken)
        {
            var appUserResult = await ValidateUserByIdAsync(userId, cancellationToken);
            if (!appUserResult.IsSuccess)
                return Result.Failure(appUserResult.Errors);

            foreach (var role in roles)
            {
                var roleExists = await _roleManager.RoleExistsAsync(role);
                if (!roleExists)
                    return Result.Failure(ApplicationErrors.IdentityErrors.RoleNotFound(role));
            }

            var identityResult = await _userManager.AddToRolesAsync(appUserResult.Value!, roles);

            if (!identityResult.Succeeded)
                return identityResult.ToResult();

            return Result.Success();
        }
        public async Task<Result> RemoveRoleFromUserAsync(Guid userId, string role, CancellationToken cancellationToken)
        {
            var userResult = await ValidateUserByIdAsync(userId, cancellationToken);
            if (!userResult.IsSuccess)
                return Result.Failure(userResult.Errors);

            var roleExists = await _roleManager.RoleExistsAsync(role);
            if (!roleExists)
                return Result.Failure(ApplicationErrors.IdentityErrors.RoleNotFound(role));

            var identityResult = await _userManager.RemoveFromRoleAsync(userResult.Value!, role);
            if (!identityResult.Succeeded)
                return identityResult.ToResult();

            return Result.Success();
        }
        public async Task<Result<IEnumerable<string>>> GetUserRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            var userResult = await ValidateUserByIdAsync(userId, cancellationToken);
            if (!userResult.IsSuccess)
                return Result<IEnumerable<string>>.Failure(userResult.Errors);

            var roles = await _userManager.GetRolesAsync(userResult.Value!);
            return Result<IEnumerable<string>>.Success(roles);
        }


        #endregion

        #region User Existence Validation Methods
        public async Task<bool> EnsureEmailExistsAsync(string email, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email);

            return user != null;
        }

        #endregion

        #region Get User Methods
        public async Task<Result<IdentityUserDto>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            var result = await ValidateUserByIdAsync(userId, cancellationToken);

            if (result.IsSuccess == false)
            {
                return Result<IdentityUserDto>.Failure(result.Errors);
            }
            var userRoles = await _userManager.GetRolesAsync(result.Value!);

            var appUser = MapToDto(result.Value!, userRoles);

            return Result<IdentityUserDto>.Success(appUser);

        }

        public async Task<Result<IdentityUserDto>> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
        {
            var result = await ValidateUserByEmail(email, cancellationToken);

            if (result.IsSuccess == false)
            {
                return Result<IdentityUserDto>.Failure(result.Errors);
            }
            var userRoles = await _userManager.GetRolesAsync(result.Value!);

            var appUser = MapToDto(result.Value!, userRoles);

            return Result<IdentityUserDto>.Success(appUser);
        }


        public async Task<Result<UserDetailsDto>> GetUserDetailsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
            {
                return Result<UserDetailsDto>.Failure(ApplicationErrors.IdentityErrors.UserNotFoundById(userId.ToString()));
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            var isActive = !user.LockoutEnd.HasValue || user.LockoutEnd <= DateTimeOffset.UtcNow;

            var userDto = new UserDetailsDto(
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email!,
                user.UserName!,
                role,
                isActive,
                user.EmailConfirmed,
                user.LastLoginDate,
                user.LockoutEnd?.DateTime);

            return Result<UserDetailsDto>.Success(userDto);
        }
        #endregion

        #region User Management Methods

        public async Task<Result<IdentityUserDto>> CreateUserAsync(
            string firstName,
            string lastName,
            string email,
            string password,
            CancellationToken cancellationToken,
            string role = Roles.User)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
                return Result<IdentityUserDto>.Failure(ApplicationErrors.IdentityErrors.EmailAlreadyExists());

            ApplicationUser applicationUser = new ApplicationUser
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                UserName = email

            };

            var identityResult = await _userManager.CreateAsync(applicationUser, password);


            if (!identityResult.Succeeded)

                return identityResult.ToResult<IdentityUserDto>();


            var addToRoleResult = await _userManager.AddToRoleAsync(applicationUser, role);

            if (!addToRoleResult.Succeeded)

                return addToRoleResult.ToResult<IdentityUserDto>();


            var identityUser = MapToDto(applicationUser, new List<string> { role });

            return Result<IdentityUserDto>.Success(identityUser);
        }

        public async Task<Result<AuthenticationResult>> CreateUserAndAuthenticateAsync(
            string firstName,
            string lastName,
            string email,
            string password,
            CancellationToken cancellationToken,
            string role = Roles.User)
        {
            var createResult = await CreateUserAsync(firstName, lastName, email, password, cancellationToken, role);

            if (!createResult.IsSuccess)
                return Result<AuthenticationResult>.Failure(createResult.Errors);

            var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .FirstAsync(u => u.Id == createResult.Value!.Id, cancellationToken);

            var userRoles = await _userManager.GetRolesAsync(user);

            var accessToken = _tokenService.GenerateAccessToken(user, userRoles);

            user.LastLoginDate = _dateTime.UTCNow;
            await _userManager.UpdateAsync(user);

            var newRefreshToken = _tokenService.GenerateRefreshToken();
            var refreshToken = RefreshToken.Create(user.Id, newRefreshToken.Item1, newRefreshToken.Item2);
            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            var authResult = new AuthenticationResult
            {
                UserId = user.Id,
                Email = user.Email!,
                UserName = user.UserName!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = userRoles,
                AccessToken = accessToken.Item1,
                AccessTokenExpiresAt = accessToken.Item2,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiresAt = refreshToken.ExpiresAtUTC
            };

            return Result<AuthenticationResult>.Success(authResult);
        }

        public async Task<Result> LockUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            var userResult = await ValidateUserByIdAsync(userId, cancellationToken);

            if (!userResult.IsSuccess)
                return Result.Failure(userResult.Errors);

            var setResult = await _userManager.SetLockoutEndDateAsync(userResult.Value!, _dateTime.UTCNow.AddYears(2));

            if (!setResult.Succeeded)
                return Result.Failure(userResult.Errors);

            return Result.Success();
        }

        public async Task<Result> UnlockUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            var userResult = await ValidateUserByIdAsync(userId, cancellationToken);

            if (!userResult.IsSuccess)
                return Result.Failure(userResult.Errors);

            var setResult = await _userManager.SetLockoutEndDateAsync(userResult.Value!, null);

            if (!setResult.Succeeded)
                return Result.Failure(userResult.Errors);

            return Result.Success();

        }

        public async Task<Result<AuthenticationResult>> AuthenticateAsync(string email, string password, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users
                .Where(X => X.Email == email)
                .Include(X => X.RefreshTokens)
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
                return Result<AuthenticationResult>.Failure(ApplicationErrors.IdentityErrors.InvalidCredentials);


            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);


            if (!result.Succeeded && !result.IsLockedOut)
                return Result<AuthenticationResult>.Failure(ApplicationErrors.IdentityErrors.InvalidCredentials);

            if (!result.Succeeded && result.IsLockedOut)
                return Result<AuthenticationResult>.Failure(ApplicationErrors.IdentityErrors.UserLockout);


            var userRoles = await _userManager.GetRolesAsync(user);

            var accessToken = _tokenService.GenerateAccessToken(user, userRoles);


            user.LastLoginDate = _dateTime.UTCNow;
            await _userManager.UpdateAsync(user);

            RefreshToken refreshToken;

            if (!user.RefreshTokens.Any(X => X.IsActive))
            {
                var newRefreshToken = _tokenService.GenerateRefreshToken();
                refreshToken = RefreshToken.Create(user.Id, newRefreshToken.Item1, newRefreshToken.Item2);
                user.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);
            }
            else
            {
                refreshToken = user.RefreshTokens.First(X => X.IsActive);
            }

            var authResult = new AuthenticationResult
            {
                UserId = user.Id,
                Email = user.Email!,
                UserName = user.UserName!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = userRoles,
                AccessToken = accessToken.Item1,
                AccessTokenExpiresAt = accessToken.Item2,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiresAt = refreshToken.ExpiresAtUTC
            };


            return Result<AuthenticationResult>.Success(authResult);


        }

        public async Task<Result<AuthenticationResult>> AuthenticateByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {


            if (string.IsNullOrEmpty(refreshToken))
                return Result<AuthenticationResult>.Failure(ApplicationErrors.IdentityErrors.InvalidToken);

            var user = await _userManager.Users
                .AsNoTracking()
                .Where(X => X.RefreshTokens
                .Any(X => X.Token == refreshToken))
                .FirstOrDefaultAsync(cancellationToken);



            if (user is null)
                return Result<AuthenticationResult>.Failure(ApplicationErrors.IdentityErrors.InvalidToken);

            var RefreshToken = user.RefreshTokens.First(x => x.Token == refreshToken);

            if (!RefreshToken.IsActive)
                return Result<AuthenticationResult>.Failure(ApplicationErrors.IdentityErrors.InvalidToken);


            var userRoles = await _userManager.GetRolesAsync(user);

            var accessToken = _tokenService.GenerateAccessToken(user, userRoles);


            var authResult = new AuthenticationResult
            {
                UserId = user.Id,
                Email = user.Email!,
                UserName = user.UserName!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = userRoles,
                AccessToken = accessToken.Item1,
                AccessTokenExpiresAt = accessToken.Item2,
                RefreshToken = RefreshToken.Token,
                RefreshTokenExpiresAt = RefreshToken.ExpiresAtUTC
            };

            return Result<AuthenticationResult>.Success(authResult);
        }

        public async Task<Result> RevokeActiveRefreshTokenAsync(Guid userId, CancellationToken cancellationToken)
        {
            if (userId == Guid.Empty)
                return Result.Failure(ApplicationErrors.IdentityErrors.UserNotFoundById(userId.ToString()));

            var user = await _userManager.Users
            .Where(X => X.Id == userId)
             .Include(X => X.RefreshTokens)
            .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return Result.Failure(ApplicationErrors.IdentityErrors.UserNotFoundById(userId.ToString()));

            if (user.RefreshTokens.Any(X => X.IsActive))
            {
                var refreshToken = user.RefreshTokens.First(X => X.IsActive);
                refreshToken.Revoke(_dateTime.UTCNow);
                await _userManager.UpdateAsync(user);
            }




            return Result.Success();
        }
        public async Task<Result> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, string confirmNewPassword, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
                return Result.Failure(ApplicationErrors.IdentityErrors.UserNotFound());

            var identityResult = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            return identityResult.ToResult();



        }



        public async Task<Result<IdentityUserDto>> UpdateUserAsync(
            Guid userId,
            string firstName,
            string lastName,
            string? newRole,
            CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
            {
                return Result<IdentityUserDto>.Failure(ApplicationErrors.IdentityErrors.UserNotFoundById(userId.ToString()));
            }

            user.FirstName = firstName;

            user.LastName = lastName;


            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                return updateResult.ToResult<IdentityUserDto>();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            var currentRole = currentRoles.FirstOrDefault();


            if (!string.IsNullOrWhiteSpace(newRole) && currentRole != newRole)
            {
                if (currentRole != null)
                {
                    await _userManager.RemoveFromRoleAsync(user, currentRole);
                }

                var roleExists = await _roleManager.RoleExistsAsync(newRole);

                if (!roleExists)
                {
                    return Result<IdentityUserDto>.Failure(ApplicationErrors.IdentityErrors.RoleNotFound(newRole));
                }

                var addRoleResult = await _userManager.AddToRoleAsync(user, newRole);


                if (!addRoleResult.Succeeded)
                {
                    return addRoleResult.ToResult<IdentityUserDto>();
                }

                currentRoles.Clear();

                currentRoles.Add(newRole);
            }

            return Result<IdentityUserDto>.Success(MapToDto(user, currentRoles));
        }

        public async Task<Result<IdentityUserDto>> UpdateUserProfileAsync(
            Guid userId,
            string firstName,
            string lastName,
            CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
            {
                return Result<IdentityUserDto>.Failure(ApplicationErrors.IdentityErrors.UserNotFoundById(userId.ToString()));
            }

            user.FirstName = firstName;
            user.LastName = lastName;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                return updateResult.ToResult<IdentityUserDto>();
            }

            var roles = await _userManager.GetRolesAsync(user);

            return Result<IdentityUserDto>.Success(MapToDto(user, roles));
        }

        public async Task<Result<AuthenticationResult>> AuthenticateByGoogleTokenAsync(string googleTokenId, CancellationToken cancellationToken = default)
        {
            GoogleJsonWebSignature.ValidationSettings validationSettings = new GoogleJsonWebSignature.ValidationSettings();

            validationSettings.Audience = new List<string>() { _configuration["Authentication:Google:ClientId"] ?? string.Empty };


            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(googleTokenId, validationSettings);

                var user = await _userManager.FindByEmailAsync(payload.Email);


                if (user == null)
                    return Result<AuthenticationResult>.Failure(ApplicationErrors.IdentityErrors.UserNotFoundByEmail);


                if (user.LockoutEnd != null && user.LockoutEnd > _dateTime.UTCNow)
                    return Result<AuthenticationResult>.Failure(ApplicationErrors.IdentityErrors.UserLockout);

                var userRoles = await _userManager.GetRolesAsync(user);

                (string accessToken, DateTime expirationDate) = _tokenService.GenerateAccessToken(user, userRoles);

                user.LastLoginDate = _dateTime.UTCNow;

                await _userManager.UpdateAsync(user);

                RefreshToken refreshToken;

                if (!user.RefreshTokens.Any(X => X.IsActive))
                {
                    var newRefreshToken = _tokenService.GenerateRefreshToken();
                    refreshToken = RefreshToken.Create(user.Id, newRefreshToken.Item1, newRefreshToken.Item2);
                    user.RefreshTokens.Add(refreshToken);
                    await _userManager.UpdateAsync(user);
                }
                else
                {
                    refreshToken = user.RefreshTokens.First(X => X.IsActive);
                }

                var authResult = new AuthenticationResult
                {
                    UserId = user.Id,
                    Email = user.Email!,
                    UserName = user.UserName!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = userRoles,
                    AccessToken = accessToken,
                    AccessTokenExpiresAt = expirationDate,
                    RefreshToken = refreshToken.Token,
                    RefreshTokenExpiresAt = refreshToken.ExpiresAtUTC
                };

                return Result<AuthenticationResult>.Success(authResult);
            }
            catch (Exception)
            {
                return Result<AuthenticationResult>.Failure(ApplicationErrors.IdentityErrors.InvalidGoogleIdToken);
            }



        }


        public async Task<Result<IdentityUserDto>> CreateUserByGoogleTokenAsync(string googleTokenId, CancellationToken cancellationToken = default)
        {
            GoogleJsonWebSignature.ValidationSettings validationSettings = new GoogleJsonWebSignature.ValidationSettings();

            validationSettings.Audience = new List<string>() { _configuration["Authentication:Google:ClientId"] ?? string.Empty };


            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(googleTokenId, validationSettings);

                var firstName = payload.Name.Split(' ')[0];

                var lastName = payload.Name.Split(' ').Length > 1 ? payload.Name.Split(' ')[1] : string.Empty;

                var email = payload.Email;

                var randomPassword = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

                ApplicationUser applicationUser = new ApplicationUser
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    EmailConfirmed = true
                };

                var createApplicationUserResult = (await _userManager.CreateAsync(applicationUser, randomPassword)).ToResult();

                if (!createApplicationUserResult.IsSuccess)
                    return Result<IdentityUserDto>.Failure(createApplicationUserResult.Errors);

                var addToRoleResult = (await _userManager.AddToRoleAsync(applicationUser, Roles.User)).ToResult();

                if (!addToRoleResult.IsSuccess)
                    return Result<IdentityUserDto>.Failure(addToRoleResult.Errors);



                return Result<IdentityUserDto>.Success(MapToDto(applicationUser, new List<string> { Roles.User }));


            }
            catch (Exception)
            {
                return Result<IdentityUserDto>.Failure(ApplicationErrors.IdentityErrors.InvalidGoogleIdToken);

            }

        }


        public async Task<Result<(string otp, string fullName)>> GenerateResetPasswordOTP(
        string email,
        CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Result<(string, string)>.Failure(ApplicationErrors.IdentityErrors.UserNotFound());
            }

            var isUserLocked = await _userManager.IsLockedOutAsync(user);

            if (isUserLocked)
            {
                return Result<(string, string)>.Failure(ApplicationErrors.IdentityErrors.UserLockout);
            }


            var key = _cache.Get<string>($"ResetPasswordOTP_{email}");

            if (!string.IsNullOrEmpty(key))
            {
                return Result<(string, string)>.Failure(ApplicationErrors.IdentityErrors.OtpCooldown());
            }
            else
            {

                await _userManager.UpdateSecurityStampAsync(user);

            }


            var otp = await _userManager.GenerateTwoFactorTokenAsync(user, "ResetPasswordOTPProvider");


            _cache.Set<string>($"ResetPasswordOTP_{email}", otp, TimeSpan.FromSeconds(30));

            return Result<(string, string)>.Success((otp, $"{user.FirstName} {user.LastName}"));
        }


        public async Task<Result<string>> VerifyResetPasswordOTP(
            string email,
            string otp,
            CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Result<string>.Failure(ApplicationErrors.IdentityErrors.UserNotFoundByEmail);
            }

            var isUserLocked = await _userManager.IsLockedOutAsync(user);

            if (isUserLocked)
            {
                return Result<string>.Failure(ApplicationErrors.IdentityErrors.UserLockout);
            }

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, "ResetPasswordOTPProvider", otp);


            if (!isValid)
            {
                await _userManager.AccessFailedAsync(user);
                return Result<string>.Failure(ApplicationErrors.IdentityErrors.InvalidOtp);
            }
            await _userManager.UpdateSecurityStampAsync(user);

            _cache.Remove($"ResetPasswordOTP_{email}");

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);


            return Result<string>.Success(resetToken);
        }


        public async Task<Result> ResetPasswordAsync(
            string email,
            string resetToken,
            string newPassword,
            CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Result.Failure(ApplicationErrors.IdentityErrors.UserNotFoundByEmail);
            }
            var resetResult = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);

            if (!resetResult.Succeeded && resetResult.Errors.Any(X => X.Code == ApplicationErrors.IdentityErrors.InvalidToken.Code))
            {
                return Result.Failure(ApplicationErrors.IdentityErrors.InvalidResetToken);
            }
            if (!resetResult.Succeeded)
            {
                return resetResult.ToResult();
            }

            return Result.Success();
        }


        #endregion

    }
}