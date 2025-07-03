
using DTC.Domain.Entities.Identity;

namespace DTC.Application.Interfaces
{
    public interface ITokenService
    {
       Task<string> GenerateJwtToken(User user);
    }
}
