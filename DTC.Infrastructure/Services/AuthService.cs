using DTC.API.DTO;
using DTC.API.ErrorHandlers;
using DTC.Domain.Entities;
using DTC.Domain.Interfaces;
using DTC.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

namespace DTC.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDataBaseContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _config;

        public AuthService(
            ApplicationDataBaseContext context,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ITokenService tokenService,
            IConfiguration config)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _config = config;
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
                Birthday = registerDTO.Birthday,
                UserName = registerDTO.Username,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, registerDTO.Password);

            await _userManager.AddToRoleAsync(user, "Member");
            if (!result.Succeeded)
                throw new HttpExeption(422, "Looks like the attempted has failed. Please check your data and try again.");

            return new UserDTO
            {
                Username = user.UserName,
                Email = user.Email,
                Token = _tokenService.GenerateJwtToken(user)
            };
        }

        public async Task<UserDTO> LoginAsync(LoginDTO login)
        {
            var user = await _userManager.FindByEmailAsync(login.Email);
            if (user is null)
                throw new HttpExeption(StatusCodes.Status401Unauthorized, "Invalid username or password");
            await _signInManager.CheckPasswordSignInAsync(user, login.Password, false);

            return new UserDTO
            {
                Username = user.UserName,
                Email = user.Email,
                Token = _tokenService.GenerateJwtToken(user)
            };
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
