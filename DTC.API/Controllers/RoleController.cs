using DTC.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DTC.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class RoleController : ControllerBase
    {
        private readonly RoleService _roleService;

        public RoleController(RoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet("my-roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _roleService.GetCurrentUserRolesAsync();
            return Ok(roles);
        }

        [HttpPost("add-role/{roleName}")]
        public async Task<IActionResult> AddRole(string roleName)
        {
            var success = await _roleService.AddRoleToCurrentUserAsync(roleName);
            return success ? Ok("Роль добавлена") : BadRequest("Ошибка при добавлении роли");
        }

        [HttpDelete("remove-role/{roleName}")]
        public async Task<IActionResult> RemoveRole(string roleName)
        {
            var success = await _roleService.RemoveRoleFromCurrentUserAsync(roleName);
            return success ? Ok("Роль удалена") : BadRequest("Ошибка при удалении роли");
        }
    }

}
