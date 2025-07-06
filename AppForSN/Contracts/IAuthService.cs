using AppForSNForUsers.DTOs;
using System.Threading.Tasks;

namespace AppForSNForUsers.Contracts
{
    public interface IAuthService
    {
        Task<UserDTO> LoginAsync(LoginDTO login);
        Task<UserDTO> RegisterAsync(RegisterDTO register);
        Task LogoutAsync();
        Task<string> GetTokenAsync();
        Task<bool> IsAuthenticatedAsync();
    }
}
