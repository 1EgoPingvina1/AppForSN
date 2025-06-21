using System.Security.Cryptography;

namespace DTC.Domain.Entities.Identity
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsActive => DateTime.UtcNow <= ExpiresAt;
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
