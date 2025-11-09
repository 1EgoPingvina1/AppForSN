namespace DTC.Domain.Entities.Identity.TwoFactor
{
    public class TwoFactorVerifyResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
