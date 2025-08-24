using DTC.Domain.Entities.Identity;

namespace DTC.Application.Interfaces.Repo
{
    public interface IRoleRepository
    {
        Task<IEnumerable<string>> GetAllRolesAsync();
        Task<User> GetCurrentUserAsync();
        Task<bool> RoleExistsAsync(string roleName);
        Task<bool> AddRoleToUserAsync(int userId, string roleName);
        Task<bool> RemoveRoleFromUserAsync(int userId, string roleName);
        Task<IList<string>> GetUserRolesAsync(int userId);
    }
}
