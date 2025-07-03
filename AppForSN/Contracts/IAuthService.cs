using AppForSNForUsers.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSNForUsers.Contracts
{
    public interface IAuthService
    {
        Task<UserDTO> LoginAsync(LoginDTO login);
        Task LogoutAsync();
        Task<string> GetTokenAsync();
        Task<bool> IsAuthenticatedAsync();
    }
}
