using DTC.Domain.Entities.Identity;

namespace DTC.Application.Interfaces.Services
{
    public interface ITokenService
    {
        Task<string> GenerateJwtToken(User user);
    }
}
