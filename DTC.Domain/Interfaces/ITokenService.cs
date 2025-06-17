using DTC.Domain.Entities;

namespace DTC.Domain.Interfaces
{
    public interface ITokenService
    {
       string GenerateJwtToken(User user);
    }
}
