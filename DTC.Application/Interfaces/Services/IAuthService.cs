using DTC.Application.DTO.Account;
using DTC.Application.DTO.Profile;
using DTC.Domain.Entities.Identity;

namespace DTC.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<TokenResponseDTO> RegisterAsync(RegisterDTO register);
        Task<TokenResponseDTO> LoginAsync(LoginDTO login);
        Task<UserProfileDTO?> GetUserProfileAsync(int userId);
        Task<User> UpdateProfileAsync(int userId, UpdateProfileDto updateDto);
        Task<TokenResponseDTO> RefreshTokenAsync();
        Task LogoutAsynс();
        Task RequestPasswordResetAsync(string email);
        Task ResetPasswordAsync(PasswordResetDTO dto);
        Task ConfirmEmailAsync(string userId, string token);
    }
}
