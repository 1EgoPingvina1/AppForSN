using DTC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DTC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDataBaseContext _context;
        public UserController(ApplicationDataBaseContext context)
        {
            _context = context;
        }
        [HttpGet("user-with-roles")]
        public async Task<IActionResult> GetUsersWithRoles()
        {
          var roles =  await _context.Users
            .AsNoTracking()
            .Select(r => new
            {
                r.Id,
                r.UserName,
                Roles = r.UserRoles.Select(r => r.Role.Name).ToList()
            }).ToListAsync();
            return Ok(roles);
        }
    }
}
