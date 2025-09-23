using DTC.Application.DTO.Account;
using DTC.Domain.Entities.Identity;

namespace DTC.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<UserDTO> RegisterAsync(RegisterDTO register);
        Task<UserDTO> LoginAsync(LoginDTO login);
        Task<TokenResponseDTO> RefreshTokenAsync();
        Task LogoutAsynс();
        Task RequestPasswordResetAsync(string email);
        Task ResetPasswordAsync(PasswordResetDTO dto);
        Task ConfirmEmailAsync(string userId, string token);
    }
}
