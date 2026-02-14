using FluxStore.Application.Common.Models;
using FluxStore.Application.Contracts.Identity;
using FluxStore.Application.Users.Queries.GetUser;
using FluxStore.Application.Users.Queries.ListUsers;
using FluxStore.Domain.Core.Primitives.Result;
using FluxStore.Domain.Enums;

public interface IIdentityService
{
    Task<Result<AuthenticationResult>> AuthenticateAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);

    Task<Result<AuthenticationResult>> AuthenticateUsingEmailOnlyAsync(
      string email,
      CancellationToken cancellationToken = default);

    Task<Result<AuthenticationResult>> AuthenticateAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);

    Task<Result> RevokeActiveRefreshToken(
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result<IdentityUserDto>> GetUserByIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result<IdentityUserDto>> GetUserByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<string>> GetUsersEmailsByRoleAsync(
    string role,
    CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<string>>> GetUserRolesByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> CreateUserAsync(
        string firstName,
        string lastName,
        string email,
        string password,
        CancellationToken cancellationToken = default,
        string role = Roles.User);

    Task<Result> ChangePasswordAsync(
        string userId,
        string currentPassword,
        string newPassword,
        string confirmNewPassword,
        CancellationToken cancellationToken = default);

    Task<Result> AddRoleToUserAsync(
        string userId,
        string role,
        CancellationToken cancellationToken = default);

    Task<Result> RemoveRoleFromUserAsync(
        string userId,
        string role,
        CancellationToken cancellationToken = default);

    Task<Result> AddRolesToUserAsync(
        string userId,
        IEnumerable<string> roles,
        CancellationToken cancellationToken = default);

    Task<bool> EnsureEmailExistsAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<Result> LockUser(
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> UnlockUser(
        string userId,
        CancellationToken cancellationToken = default);

    // User Management Extensions
    Task<PaginatedList<UserListItemDto>> ListUsersAsync(
        string? searchTerm,
        string? role,
        bool? isActive,
        string? SortBy,
        bool SortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<Result<UserDetailsDto>> GetUserDetailsAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateUserAsync(
        string userId,
        string firstName,
        string lastName,
        string role,
        CancellationToken cancellationToken = default);

    Task<Result<(string otp, string fullName)>> GenerateResetPasswordOTP(
        string email,
        CancellationToken cancellationToken = default);



    Task<Result<string>> VerifyResetPasswordOTP(
        string email,
        string otp,
        CancellationToken cancellationToken = default);

    Task<Result> ResetPasswordAsync(
     string email,
     string resetToken,
     string newPassword,
     CancellationToken cancellationToken = default);

    Task<Result<(string fullName, string emailAddress)>> AuthenticateWithGoolge(
        string clientId,
        CancellationToken cancellationToken = default);
}
