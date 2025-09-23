using DTC.Domain.Entities.Identity;

namespace DTC.Application.Interfaces.Services
{
    public interface ITokenService
    {
        Task<string> GenerateJwtToken(User user);
        Task<RefreshToken> GenerateRefreshToken(User user);
        string HashToken(string token);
    }
}
