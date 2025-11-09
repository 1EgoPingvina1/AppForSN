using DTC.Domain.Entities.Identity;

namespace DTC.Application.Interfaces.Services.TwoFactoryAuth
{
    public interface ITwoFactoryAuthorization
    {
        Task<(string Secret, string OtpAuthUri)> GenerateSecretAsync(string userEmail, string issuer);
        bool VerifyCodeAsync(string secret, string code);
        Task<string[]> GenerateRecoveryCodesAsync(int count = 10);
        Task<bool> GetStatus();
        Task EnableTwoFactorAsync(User user, string secret, string[] recoveryCodes);
    }
}
