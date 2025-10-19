using System.ComponentModel.DataAnnotations.Schema;

namespace DTC.Domain.Entities.Identity.TwoFactor
{
    [NotMapped]
    public class TwoFactorSetupResponse
    {
        public bool Success { get; set; }
        public string? Secret { get; set; }
        public string? OtpAuthUri { get; set; }
        public string[] RecoveryCodes { get; set; } = Array.Empty<string>();
        public string? ManualEntryKey { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
