using DTC.Application.Interfaces;
using DTC.Domain.Entities.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace DTC.Infrastructure.Services
{
    public class RoleService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IRoleRepository _roleRepository;
        private readonly UserManager<User> _userManager;

        public RoleService(IHttpContextAccessor contextAccessor, IRoleRepository roleRepository, UserManager<User> userManager)
        {
            _contextAccessor = contextAccessor;
            _roleRepository = roleRepository;
            _userManager = userManager;
        }

        public async Task GetAllRoles() => await _roleRepository.GetAllRolesAsync();

        

        public async Task<IEnumerable<string>> GetCurrentUserRolesAsync()
        {
            var user = await _roleRepository.GetCurrentUserAsync();
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<bool> AddRoleToCurrentUserAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentException("Имя роли не может быть пустым");

            var user = await _roleRepository.GetCurrentUserAsync();

            if (await _userManager.IsInRoleAsync(user, roleName))
                return true;

            var result = await _userManager.AddToRoleAsync(user, roleName);
            return result.Succeeded;
        }

        public async Task<bool> RemoveRoleFromCurrentUserAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentException("Имя роли не может быть пустым");

            var user = await _roleRepository.GetCurrentUserAsync();

            if (!await _userManager.IsInRoleAsync(user, roleName))
                return true;

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            return result.Succeeded;
        }
    }
}
