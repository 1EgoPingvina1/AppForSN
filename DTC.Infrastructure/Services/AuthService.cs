using DTC.Application.DTO;
using DTC.Application.ErrorHandlers;
using DTC.Application.Interfaces;
using DTC.Domain.Entities.Identity;
using DTC.Infrastructure.Data;
using DTC.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace DTC.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDataBaseContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailService _emailService;

        public AuthService(
            ApplicationDataBaseContext context,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ITokenService tokenService,
            IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _emailService = emailService;
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

            await _userManager.AddToRoleAsync(user, "User");
            if (!result.Succeeded)
                throw new HttpExeption(422, "Looks like the attempted has failed. Please check your data and try again.");

            var refreshToken = await GenerateRefreshToken(user);

            return new UserDTO
            {
                Username = user.UserName,
                Token = await _tokenService.GenerateJwtToken(user),
                RefreshToken = refreshToken.Token
            };
        }

        public async Task<TokenResponseDTO> RefreshTokenAsync(RefreshTokenDTO dto)
        {
            var token = await _context.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(t => t.Token == dto.Token);

            if (token == null || token.ExpiresAt < DateTime.UtcNow)
                throw new HttpExeption(401, "Refresh токен недействителен");

            var accessToken = await _tokenService.GenerateJwtToken(token.User);
            var newRefreshToken = await GenerateRefreshToken(token.User);

            _context.RefreshTokens.Remove(token);
            await _context.SaveChangesAsync();

            return new TokenResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken.Token,
            };
        }

        public async Task<UserDTO> LoginAsync(LoginDTO login)
        {
            var user = await _userManager.FindByNameAsync(login.Username);
            if (user is null)
                throw new HttpExeption(401, "Invalid username or password");

            var result = await _signInManager.CheckPasswordSignInAsync(user, login.Password, false);
            if (!result.Succeeded)
                throw new HttpExeption(401, "Invalid username or password");

            var refreshToken = await GenerateRefreshToken(user);

            return new UserDTO
            {
                Username = user.UserName,
                Token = await _tokenService.GenerateJwtToken(user),
                RefreshToken = refreshToken.Token
            };
        }

        public async Task LogoutAsync(string refreshToken)
        {
            var token = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken);
            if (token != null)
            {
                _context.RefreshTokens.Remove(token);
                await _context.SaveChangesAsync();
            }
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

        public async Task<RefreshToken> GenerateRefreshToken(User user)
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                UserId = user.Id
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return refreshToken;
        }
    }
}
