
using DTC.Domain.Entities.Identity;

namespace DTC.Application.Interfaces
{
    public interface ITokenService
    {
       string GenerateJwtToken(User user);
    }
}
