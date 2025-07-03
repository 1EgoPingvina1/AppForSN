
using DTC.Application.DTO;

namespace DTC.Application.Interfaces
{
    public interface IAuthService
    {
        Task<UserDTO> RegisterAsync(RegisterDTO register);
        Task<UserDTO> LoginAsync(LoginDTO login);
        //Task<AuthResult> RefreshTokenAsync(string token, string refreshToken);
        //Task<Result> RevokeTokenAsync(string refreshToken);
        //Task<Result> RequestPasswordResetAsync(string email);
        //Task<AuthResult> ResetPasswordAsync(string token, string newPassword);
        //Task<Result> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
    }
}
