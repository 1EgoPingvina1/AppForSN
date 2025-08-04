
using DTC.Application.DTO;
using DTC.Domain.Entities.Identity;

namespace DTC.Application.Interfaces
{
    public interface IAuthService
    {
        Task<UserDTO> RegisterAsync(RegisterDTO register);
        Task<UserDTO> LoginAsync(LoginDTO login);
        Task<TokenResponseDTO> RefreshTokenAsync(RefreshTokenDTO refreshToken);
        Task LogoutAsync(string refreshToken);
        Task RequestPasswordResetAsync(string email);
        Task ResetPasswordAsync(PasswordResetDTO dto);
        Task ConfirmEmailAsync(string userId, string token);
        Task<RefreshToken> GenerateRefreshToken(User user);
    }
}
