using System.Security.Claims;
using OpenSpaceBooking.BLL.Dtos;

namespace OpenSpaceBooking.BLL.Interfaces;

public interface IAuthService
{
    Task<AuthCallback> LoginAsync(string email, string password);
    Task<AuthCallback> RegisterAsync(RegisterDto model);
    Task<string> GenerateResetPasswordTokenAsync(string email);
    Task<string> GenerateEmailConfirmationTokenAsync(string email);
    Task ChangePassword(string newPassword, string email);
    Task ResetPasswordAsync(ResetPasswordDto model);
    Task ConfirmEmailAsync(string token, string email);
    Task EditUserAsync(UserEditDto model, string userId);
    Task DeleteAccountAsync(string userId);
}