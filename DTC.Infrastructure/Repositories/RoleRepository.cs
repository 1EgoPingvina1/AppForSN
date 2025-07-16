using DTC.Application.Interfaces;
using DTC.Domain.Entities.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DTC.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly RoleManager<Role> roleManager;
        private readonly UserManager<User> userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public RoleRepository(RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }


        public async Task<User> GetCurrentUserAsync()
        {
            var userId = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Пользователь не авторизован");

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                throw new Exception("Пользователь не найден");

            return user;
        }

        public async Task<IEnumerable<string>> GetAllRolesAsync()
        {
            return await roleManager.Roles.Select(r => r.Name).ToListAsync();
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            return await roleManager.RoleExistsAsync(roleName);
        }

        public async Task<bool> AddRoleToUserAsync(int userId, string roleName)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null || !await roleManager.RoleExistsAsync(roleName))
                return false;

            var result = await userManager.AddToRoleAsync(user, roleName);
            return result.Succeeded;
        }

        public async Task<bool> RemoveRoleFromUserAsync(int userId, string roleName)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null || !await roleManager.RoleExistsAsync(roleName))
                return false;

            var result = await userManager.RemoveFromRoleAsync(user, roleName);
            return result.Succeeded;
        }

        public async Task<IList<string>> GetUserRolesAsync(int userId)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            return user != null ? await userManager.GetRolesAsync(user) : new List<string>();
        }
    }
}
