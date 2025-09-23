using DTC.Application.DTO.Account;
using DTC.Application.ErrorHandlers;
using DTC.Application.Interfaces.Services;
using DTC.Domain.Entities.Identity;
using DTC.Infrastructure.Data;
using DTC.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace DTC.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDataBaseContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _HttpContextAccessor;

        public AuthService(
            ApplicationDataBaseContext context,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ITokenService tokenService,
            IEmailService emailService,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _emailService = emailService;
            _configuration = configuration;
            _HttpContextAccessor = httpContextAccessor;
        }

        public async Task<UserDTO> RegisterAsync(RegisterDTO registerDTO)
        {
            var user = new User
            {
                FirstName = registerDTO.FirstName,
                SecondName = registerDTO.SecondName,
                LastName = registerDTO.LastName,
                Gender = registerDTO.Gender,
                IsAuthor = registerDTO.IsAuthor,
                Birthday = registerDTO.Birthday.ToUniversalTime(),
                UserName = registerDTO.Username,
                EmailConfirmed = false,
            };

            var result = await _userManager.CreateAsync(user, registerDTO.Password);
            if (!result.Succeeded) throw new HttpExeption(422, "Не удалось создать пользователя!");
            var roleResult = await _userManager.AddToRoleAsync(user, "User");
            if (!roleResult.Succeeded)
                throw new HttpExeption(422, "Ошибка при назначении роли пользователю");

            return new UserDTO
            {
                Username = user.UserName,
                Token = await _tokenService.GenerateJwtToken(user),
                RefreshToken = (await _tokenService.GenerateRefreshToken(user)).Token
            };
        }

        public async Task<TokenResponseDTO> RefreshTokenAsync()
        {
            var request = _HttpContextAccessor.HttpContext.Request;
            var rawToken = request.Cookies["refresh_token"];

            if (string.IsNullOrEmpty(rawToken))
                throw new HttpExeption(401, "Refresh токен отсутствует");

            var hashed = _tokenService.HashToken(rawToken);

            var token = await _context.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(t => t.Token == hashed);

            if (token == null || token.ExpiresAt < DateTime.UtcNow)
                throw new HttpExeption(401, "Refresh токен недействителен");

            var accessToken = await _tokenService.GenerateJwtToken(token.User);
            var newRefreshToken = await _tokenService.GenerateRefreshToken(token.User);

            _context.RefreshTokens.Remove(token);
            await _context.SaveChangesAsync();

            var response = _HttpContextAccessor.HttpContext.Response;
            response.Cookies.Append("refresh_token", newRefreshToken.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = newRefreshToken.ExpiresAt
            });

            return new TokenResponseDTO
            {
                AccessToken = accessToken
            };
        }
        public async Task<UserDTO> LoginAsync(LoginDTO login)
        {
            var user = await _userManager.FindByNameAsync(login.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, login.Password))
                throw new HttpExeption(401, "Invalid username or password");

            var jwt = await _tokenService.GenerateJwtToken(user);
            var refreshToken = await _tokenService.GenerateRefreshToken(user);

            var response = _HttpContextAccessor.HttpContext.Response;

            response.Cookies.Append("access_token", jwt, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(15)
            });

            response.Cookies.Append("refresh_token", refreshToken.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = refreshToken.ExpiresAt
            });

            return new UserDTO
            {
                Token = jwt,
                RefreshToken = refreshToken.Token
            };
        }

        public async Task LogoutAsynс()
        {
            var request = _HttpContextAccessor.HttpContext.Request;
            var rawToken = request.Cookies["refresh_token"];
            if (string.IsNullOrEmpty(rawToken))
                throw new HttpExeption(404, "Refresh token not found");

            var hashed = _tokenService.HashToken(rawToken);
            var token = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == hashed);

            if (token != null)
            {
                _context.RefreshTokens.Remove(token);
                await _context.SaveChangesAsync();
            }

            var response = _HttpContextAccessor.HttpContext.Response;
            response.Cookies.Delete("refresh_token");
            response.Cookies.Delete("access_token");
        }

        public async Task RequestPasswordResetAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var link = $"https://yourapp.com/reset-password?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";

            await _emailService.SendAsync(email, "Сброс пароля", $"Сбросьте пароль по ссылке: {link}");
        }

        public async Task ResetPasswordAsync(PasswordResetDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                throw new HttpExeption(404, "Пользователь не найден");

            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
            if (!result.Succeeded)
                throw new HttpExeption(400, "Ошибка сброса пароля");
        }

        public async Task ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new HttpExeption(404, "Пользователь не найден");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
                throw new HttpExeption(400, "Подтверждение не удалось");
        }

        

    }
}
