using DTC.Domain.Entities.Identity;

namespace DTC.Domain.Interfaces
{
    public interface ITokenService
    {
       string GenerateJwtToken(User user);
    }
}
