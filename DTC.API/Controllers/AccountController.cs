using DTC.Application.DTO.Account;
using DTC.Application.Interfaces.Services;
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

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDTO>> RefreshToken(RefreshTokenDTO dto) => Ok(await _authService.RefreshTokenAsync(dto));

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutDTO dto)
        {
            await _authService.LogoutAsync(dto.RefreshToken);
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
