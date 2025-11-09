namespace DTC.Domain.Entities.Identity.TwoFactor
{
    public class TwoFactorVerifyRequest
    {
        public string Code { get; set; } = string.Empty;
        public string Secret {  get; set; } = string.Empty;
    }
}
