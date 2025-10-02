using DTC.Application.DTO.Account;
using DTC.Application.DTO.Profile;
using DTC.Application.Interfaces.Services;
using DTC.Domain.Entities.Identity;
using DTC.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DTC.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly UserManager<User> _userManager;

        public AccountController(IAuthService authService, UserManager<User> userManager)
        {
            _authService = authService;
            _userManager = userManager;
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO login) => Ok(await _authService.LoginAsync(login));

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Unauthorized();

            return Ok(new
            {
                id = user.Id,
                username = user.UserName,
                email = user.Email
            });
        }


        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO register) => Ok(await _authService.RegisterAsync(register));

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDTO>> RefreshToken() => Ok(await _authService.RefreshTokenAsync());

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsynс();
            return NoContent();
        }

        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequestDTO dto)
        {
            await _authService.RequestPasswordResetAsync(dto.Email);
            return Ok(new { message = "If an account with that email exists, we have sent password reset instructions." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetDTO dto)
        {
            await _authService.ResetPasswordAsync(dto);
            return Ok(new { message = "Password reset successful." });
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            await _authService.ConfirmEmailAsync(userId, token);
            return Ok(new { message = "Email confirmed." });
        }
    }
}
