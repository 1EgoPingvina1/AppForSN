using DTC.Application.DTO;
using DTC.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DTC.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO login) => Ok(await _authService.LoginAsync(login));

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO register) => Ok(await _authService.RegisterAsync(register));
    }
}
