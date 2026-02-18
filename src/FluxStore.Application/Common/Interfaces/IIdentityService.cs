using FluxStore.Application.Contracts.Identity;
using FluxStore.Application.Users.Queries.GetUser;
using FluxStore.Domain.Core.Primitives.Result;
using AuthenticationResult = FluxStore.Application.Contracts.Identity.AuthenticationResult;

public interface IIdentityService
{
    Task<Result> AddRolesToUserAsync(Guid userId, IEnumerable<string> roles, CancellationToken cancellationToken);
    Task<Result> AddRoleToUserAsync(Guid userId, string role, CancellationToken cancellationToken);
    Task<Result<AuthenticationResult>> AuthenticateAsync(string email, string password, CancellationToken cancellationToken);
    Task<Result<AuthenticationResult>> AuthenticateByGoogleTokenAsync(string googleTokenId, CancellationToken cancellationToken = default);
    Task<Result<AuthenticationResult>> AuthenticateByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
    Task<Result> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, string confirmNewPassword, CancellationToken cancellationToken);
    Task<Result<IdentityUserDto>> CreateUserAsync(string firstName, string lastName, string email, string password, CancellationToken cancellationToken, string role = "User");
    Task<Result<AuthenticationResult>> CreateUserAndAuthenticateAsync(string firstName, string lastName, string email, string password, CancellationToken cancellationToken, string role = "User");
    Task<Result<IdentityUserDto>> CreateUserByGoogleTokenAsync(string googleTokenId, CancellationToken cancellationToken = default);
    Task<bool> EnsureEmailExistsAsync(string email, CancellationToken cancellationToken);
    Task<Result<(string otp, string fullName)>> GenerateResetPasswordOTP(string email, CancellationToken cancellationToken = default);
    Task<Result<IdentityUserDto>> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
    Task<Result<IdentityUserDto>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<Result<UserDetailsDto>> GetUserDetailsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<string>>> GetUserRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<Result> LockUserAsync(Guid userId, CancellationToken cancellationToken);
    Task<Result> RemoveRoleFromUserAsync(Guid userId, string role, CancellationToken cancellationToken);
    Task<Result> ResetPasswordAsync(string email, string resetToken, string newPassword, CancellationToken cancellationToken = default);
    Task<Result> RevokeActiveRefreshTokenAsync(Guid userId, CancellationToken cancellationToken);
    Task<Result> UnlockUserAsync(Guid userId, CancellationToken cancellationToken);
    Task<Result<IdentityUserDto>> UpdateUserAsync(Guid userId, string firstName, string lastName, string? newRole, CancellationToken cancellationToken = default);
    Task<Result<IdentityUserDto>> UpdateUserProfileAsync(Guid userId, string firstName, string lastName, CancellationToken cancellationToken = default);
    Task<Result<string>> VerifyResetPasswordOTP(string email, string otp, CancellationToken cancellationToken = default);
}
