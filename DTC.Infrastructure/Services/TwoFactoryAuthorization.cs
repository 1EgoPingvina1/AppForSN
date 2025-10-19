using DTC.Application.ErrorHandlers;
using DTC.Application.Interfaces.Services.TwoFactoryAuth;
using DTC.Domain.Entities.Identity;
using DTC.Infrastructure.Data;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OtpNet;
using QRCoder;
using System.Security.Claims;

namespace DTC.Infrastructure.Services
{
    public class TwoFactoryAuthorization : ITwoFactoryAuthorization
    {
        private readonly ApplicationDataBaseContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;
        private readonly IDataProtector _securityTokens;

        public TwoFactoryAuthorization(IDataProtectionProvider provider,ApplicationDataBaseContext context, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _securityTokens = provider.CreateProtector("TwoFactorSecretProtector");
        }
        public async Task<(string Secret, string OtpAuthUri)> GenerateSecretAsync(string userEmail, string issuer)
        {
            var key = KeyGeneration.GenerateRandomKey(20); 
            var base32Secret = Base32Encoding.ToString(key);

            var otpAuthUri = $"otpauth://totp/{Uri.EscapeDataString(issuer)}:{Uri.EscapeDataString(userEmail)}?" +
                            $"secret={base32Secret}&issuer={Uri.EscapeDataString(issuer)}&digits=6&algorithm=SHA1&period=30";

            return await Task.FromResult((base32Secret, otpAuthUri));
        }

        public bool VerifyCodeAsync(string secret, string code)
        {
            var totp = new Totp(Base32Encoding.ToBytes(secret));
            return totp.VerifyTotp(code, out _, new VerificationWindow(previous: 1, future: 1));
        }

        public async Task EnableTwoFactorAsync(User user, string secret, string[] recoveryCodes)
        {
            user.TwoFactorSecret = _securityTokens.Protect(secret);
            user.RecoveryCodes = _securityTokens.Protect(System.Text.Json.JsonSerializer.Serialize(recoveryCodes));
            user.TwoFactorCreatedAt = DateTime.UtcNow;
            user.TwoFactorEnabled = true;

            await _userManager.UpdateAsync(user);
        }

        public async Task<bool> GetStatus()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user.FindFirst("sub")?.Value ?? throw new HttpExeption(401, "Unauthirized");

            if (string.IsNullOrEmpty(userId))
                throw new HttpExeption(401, "Unautorized");

            var me = await _userManager.FindByIdAsync(userId);
            if (me == null)
                throw new HttpExeption(404, "Not found");

            return await _userManager.GetTwoFactorEnabledAsync(me);
            
        }

        public async Task<string[]> GenerateRecoveryCodesAsync(int count = 5)
        {
            var codes = new List<string>();
            for (int i = 0; i < count; i++)
            {
                var code = GenerateRecoveryCode();
                codes.Add(code);
            }
            return codes.ToArray();
        }

        private string GenerateRecoveryCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 10)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
